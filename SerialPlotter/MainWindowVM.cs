using GalaSoft.MvvmLight.CommandWpf;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using SerialPlotter.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace SerialPlotter
{
    public class MainWindowVM : INotifyPropertyChanged
    {

        // Commands
        public ICommand connectButtonCommand { get; private set; }
        public ICommand closeConnectionButtonCommand { get; private set; }
        public ICommand createDataModelCommand { get; private set; }

        public ICommand saveSeriesDetails { get; private set; }
        public ICommand deleteSeriesDetails { get; private set; }


        private bool isActiveSettings;
        public bool IsActiveSettings
        {
            get { return isActiveSettings; }
            set
            {
                if (isActiveSettings != value)
                {
                    isActiveSettings = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsActiveSettings"));

                }
            }
        }




        // Properties


        public SettingsModel SettingsModel { get; set; }


        public List<int> BaudRateList { get; set; }
        public List<string> ComPortList { get; set; }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> MultiController { get; set; }

        public List<ColorInfo> AllColors { get; set; }

        // Data Models Configs.

        private int dataModelsCount;

        public int DataModelsCount
        {
            get { return dataModelsCount; }
            set
            {
                if (dataModelsCount != value)
                {
                    dataModelsCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DataModelsCount"));

                }
            }
        }

        public string DataModelSeperator { get; set; }


        public ObservableCollection<DataModel> DataModels { get; set; }



        public MainWindowVM()
        {
            IsActiveSettings = true;
            // Relays
            connectButtonCommand = new RelayCommand(ConnectSerialPort);
            closeConnectionButtonCommand = new RelayCommand(CloseConnection);
            createDataModelCommand = new RelayCommand(CreateDataModels);
            saveSeriesDetails = new RelayCommand<DataModel>(SaveSeriesDetails);
            deleteSeriesDetails = new RelayCommand<DataModel>(DeleteSeriesDetails);


            // Lists
            BaudRateList = new List<int> { 9600, 115200 };
            ComPortList = SerialPort.GetPortNames().ToList();

            GetColorList();
            GetSettingsModel();
            GetSavedDataModels();


            MultiController = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            MultiController.Range.MinimumY = 0;
            MultiController.Range.MaximumY = 1080;
            MultiController.Range.MaximumX = 10;
            MultiController.Range.AutoY = true;

            CreateWpfGraphDataSeries();

        }


        private void SaveSettingsModel()
        {
            DataModelProvider.Instance.SaveSettings(SettingsModel);
        }

        private void GetSettingsModel()
        {
            SettingsModel = DataModelProvider.Instance.ReadSettings();

        }
        private void SaveSeriesDetails(DataModel dataModel)
        {
            if (dataModel != null)
            {
                var matchedColor = AllColors.Where(x => x.ColorName == dataModel.ColorInfo.ColorName).FirstOrDefault();
                dataModel.ColorInfo.Color = matchedColor.Color;
                DataModelProvider.Instance.SaveSelectedDataModel(dataModel);
                Console.WriteLine(dataModel);
                GetSavedDataModels();
                CreateWpfGraphDataSeries();
            }
        }

        private void DeleteSeriesDetails(DataModel dataModel)
        {

            if (dataModel != null)
            {
                DataModelProvider.Instance.DeleteSelectedDataModel(dataModel);
                GetSavedDataModels();
                CreateWpfGraphDataSeries();
            }


        }

        private void GetSavedDataModels()
        {
            DataModels = DataModelProvider.Instance.ReadDataModels();
            DataModelsCount = DataModels.Count;

        }


        private void CreateDataModels()
        {
            // Clear(DataModels);
            // TODO: Create One by One and Show the DataModels Count 


            DataModelsCount++;
            DataModel dataModel = new DataModel();
            dataModel.Id = DataModelsCount;
            dataModel.SeriesName = "VariableName " + DataModelsCount;
            DataModelProvider.Instance.DataModels.Add(dataModel);
            DataModelProvider.Instance.SaveDataModels();
            GetSavedDataModels();
            CreateWpfGraphDataSeries();

        }

        public void CreateWpfGraphDataSeries()
        {
            if (MultiController.DataSeriesCollection != null)
            {
                MultiController.Clear();
                MultiController.DataSeriesCollection.Clear();
                foreach (var dataModel in DataModels)
                {

                    MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
                    {
                        Name = dataModel.SeriesName,
                        Stroke = dataModel.ColorInfo.Color

                    });

                }
            }

        }


        public void CloseConnection()
        {
            SerialConnection.CloseConnection();
            IsActiveSettings = true;
        }


        // double counter = 0;
        public void ConnectSerialPort()
        {
            SerialConnection.SerialPortName = SettingsModel.SerialPort;
            SerialConnection.SerialBaudRate = SettingsModel.BaudRate;

            if (!SerialConnection.IsConnected())
            {
                IsActiveSettings = false;
                SerialConnection.CreateConnection();
                Console.WriteLine("Connected");


                //MultiController.Clear();
                Stopwatch watch = new Stopwatch();
                watch.Start();

                Task.Factory.StartNew(() =>
                {

                    while (SerialConnection.IsConnected())
                    {

                        try
                        {

                            string data = SerialConnection.SerialPort.ReadLine();
                            data = data.Replace(".", ",");

                            string[] dataIn = data.Replace('\n', '\0').Split(' ');

                            if (dataIn.Length > 1)
                            {
                                List<DoubleDataPoint> yValues = new List<DoubleDataPoint>();
                                List<DoubleDataPoint> xValues = new List<DoubleDataPoint>();

                                    // var x = watch.Elapsed;
                                    for (int i = 1; i < dataIn.Length - 1; i++)
                                {
                                    double.TryParse(dataIn[0], out double x);
                                    xValues.Add(x);
                                    double.TryParse(dataIn[i], out double value);
                                    yValues.Add(value);
                                        // counter++;

                                    }
                                MultiController.PushData(xValues, yValues);
                            }


                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message);
                        }

                    }
                    watch.Stop();
                    IsActiveSettings = true;


                });

            }

        }

        void Clear<T>(ObservableCollection<T> list)
        {
            list.Clear();
        }




        private void GetColorList()
        {
            AllColors = new List<ColorInfo>();
            PropertyInfo[] property = typeof(Colors).GetProperties();

            for (int i = 0; i < property.Length; i++)
            {
                ColorInfo colorInfo = new ColorInfo();
                colorInfo.SetColorInfo(property[i].Name, (Color)(property[i].GetValue(null, null)));
                AllColors.Add(colorInfo);

            }

        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);

        }


       

    }

}
