using GalaSoft.MvvmLight.CommandWpf;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
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
        public ICommand saveCurrentSettingsButtonCommand { get; private set; }


        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsConnected"));

                }
            }
        }


        // Properties

        public List<int> BaudRateList { get; set; }
        public List<string> ComPortList { get; set; }



        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> MultiController { get; set; }

        public List<ColorInfo> AllColors { get; set; }

        private SettingsModel settingsModel;

        public SettingsModel SettingsModel
        {
            get { return settingsModel; }
            set {

                if (settingsModel != value)
                {
                    settingsModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SettingsModel"));
                  

                }
            }
        }



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
            
            IsConnected = true;
           
            // Relays
            connectButtonCommand = new RelayCommand(ConnectSerialPort);
            closeConnectionButtonCommand = new RelayCommand(CloseConnection);
            createDataModelCommand = new RelayCommand(CreateDataModels);
            saveCurrentSettingsButtonCommand = new RelayCommand(SaveCurrentSettings);
            saveSeriesDetails = new RelayCommand<DataModel>(SaveSeriesDetails);
            deleteSeriesDetails = new RelayCommand<DataModel>(DeleteSeriesDetails);


            // Lists
            BaudRateList = new List<int> { 9600, 115200 };
           
            ComPortList = new List<string>();
            ComPortList = SerialPort.GetPortNames().ToList();

            GetColorList();
            GetSettingsModel();
            GetSavedDataModels();


            MultiController = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            MultiController.Renderer = new ScrollingLineRenderer<WpfGraphDataSeries>();
            MultiController.Surface = new WpfGraphSurface();

            MultiController.Range.MinimumY = SettingsModel.YMax;
            MultiController.Range.MaximumY = SettingsModel.YMax;
            MultiController.Range.MaximumX = SettingsModel.Duration;
            MultiController.Range.AutoY = true;
            
            //MultiController.RefreshRate = TimeSpan.FromMilliseconds(5);

            CreateWpfGraphDataSeries();

        }

        private void SettingsModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            MultiController.Range.MaximumX = SettingsModel.Duration;
            MultiController.Range.AutoY = SettingsModel.AutoRange;
            MultiController.Range.MinimumY = SettingsModel.YMin;
            MultiController.Range.MaximumY = SettingsModel.YMax;
            SaveCurrentSettings();


        }

        private void SaveCurrentSettings()
        {
            DataModelProvider.Instance.SaveSettings(SettingsModel);
        }

        private void GetSettingsModel()
        {
            SettingsModel = DataModelProvider.Instance.ReadSettings();
            SettingsModel.PropertyChanged += SettingsModel_PropertyChanged;

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
            dataModel.SeriesName = "Var " + DataModelsCount;
            dataModel.ColorInfo.SetRandomColor(DataModelsCount);
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
            IsConnected = true;
        }


        // double counter = 0;
        public void ConnectSerialPort()
        {
            SerialConnection.SerialPortName = SettingsModel.SerialPort;
            SerialConnection.SerialBaudRate = SettingsModel.BaudRate;

            if (!SerialConnection.IsConnected())
            {
                IsConnected = false;
                MultiController.Clear();
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
                                for (int i = 1; i < dataIn.Length; i++)
                                {
                                    double.TryParse(dataIn[0], out double x);
                                    xValues.Add(x / 1000);
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
                    IsConnected = true;


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
