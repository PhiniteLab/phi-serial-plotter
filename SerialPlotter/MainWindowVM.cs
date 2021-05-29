using GalaSoft.MvvmLight.CommandWpf;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using SerialPlotter.Core.Models;
using SerialPlotter.Core.Provider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;


namespace SerialPlotter
{
    public class MainWindowVM : INotifyPropertyChanged
    {


        // Private variables
        private string startSave;
        private bool isConnected;
        private bool saveDataPointActive;
        private int dataModelsCount;
        private SettingsModel settingsModel;
        private double timeCounter = 0;


        // Commands
        public ICommand connectButtonCommand { get; private set; }
        public ICommand closeConnectionButtonCommand { get; private set; }
        public ICommand createDataModelCommand { get; private set; }
        public ICommand saveSeriesDetails { get; private set; }
        public ICommand deleteSeriesDetails { get; private set; }
        public ICommand saveCurrentSettingsButtonCommand { get; private set; }
        public ICommand selectOutputFolderCommand { get; private set; }
        public ICommand startSaveDataCommand { get; private set; }


        // Properties
        public string StartSave
        {
            get { return startSave; }
            set
            {
                if (startSave != value)
                {
                    startSave = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StartSave"));

                }
            }
        }
        public bool SaveDataPointActive
        {
            get { return saveDataPointActive; }
            set
            {
                if (saveDataPointActive != value)
                {
                    saveDataPointActive = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SaveDataPointActive"));
                    if (saveDataPointActive) StartSave = "Stop Saving"; else StartSave = "Start Saving";

                }
            }
        }
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
        public List<int> BaudRateList { get; set; }
        public List<string> ComPortList { get; set; }
        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> MultiController { get; set; }
        public List<ColorInfo> AllColors { get; set; }
        public SettingsModel SettingsModel
        {
            get { return settingsModel; }
            set
            {
                if (settingsModel != value)
                {
                    settingsModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SettingsModel"));
                }
            }
        }
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
        public ObservableCollection<SeriesModel> DataModels { get; set; }


        public MainWindowVM()
        {
            InitializeCommands();
            InitializeList();
            GetColorList();
            GetSettingsModel();
            GetSavedDataModels();
            InitializeController();
            CreateWpfGraphDataSeries();
        }


        private void InitializeCommands()
        {
            StartSave = "Start Saving";
            IsConnected = false;
            SaveDataPointActive = false;

            // Relays
            connectButtonCommand = new RelayCommand(ConnectSerialPort);
            closeConnectionButtonCommand = new RelayCommand(CloseConnection);
            createDataModelCommand = new RelayCommand(CreateDataModels);
            saveCurrentSettingsButtonCommand = new RelayCommand(SaveCurrentSettings);
            selectOutputFolderCommand = new RelayCommand(SelectOutputFolder);
            startSaveDataCommand = new RelayCommand(SaveDataPoint);
            saveSeriesDetails = new RelayCommand<SeriesModel>(SaveSeriesDetails);
            deleteSeriesDetails = new RelayCommand<SeriesModel>(DeleteSeriesDetails);

        }

        private void InitializeList()
        {
            // Lists
            BaudRateList = new List<int> { 9600, 115200 };
            ComPortList = new List<string>();
            ComPortList = SerialPort.GetPortNames().ToList();
        }

        private void InitializeController()
        {
            MultiController = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            MultiController.Renderer = new ScrollingLineRenderer<WpfGraphDataSeries>();
            MultiController.Surface = new WpfGraphSurface();
            MultiController.Range.MinimumY = SettingsModel.YMax;
            MultiController.Range.MaximumY = SettingsModel.YMax;
            MultiController.Range.MaximumX = SettingsModel.Duration;
            MultiController.Range.AutoY = true;
        }

        private void SaveDataPoint()
        {
            SaveDataPointActive = !SaveDataPointActive;
            if (SaveDataPointActive) FileManager.Instance.CreateOutputFolder(SettingsModel);
        }

        private void SelectOutputFolder()
        {
            SettingsModel.OutputFolderPath = FileManager.Instance.SelectOutputFolder();
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
        private void SaveSeriesDetails(SeriesModel dataModel)
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

        private void DeleteSeriesDetails(SeriesModel dataModel)
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
            DataModelsCount++;
            SeriesModel dataModel = new SeriesModel();
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
            IsConnected = false;
            SaveDataPointActive = false;
        }


      
        public void ConnectSerialPort()
        {
            SerialConnection.SerialPortName = SettingsModel.SerialPort;
            SerialConnection.SerialBaudRate = SettingsModel.BaudRate;

            if (!SerialConnection.IsConnected())
            {
                IsConnected = true;
                MultiController.Clear();
                SerialConnection.CreateConnection();
            
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

                               
                               for (int i = 0; i < dataIn.Length; i++)
                               {

                                   double time = 0;

                                   if (SettingsModel.TimeMode == TimeMode.TimeFromComputer)
                                   {
                                       time = timeCounter / 1000;
                                       timeCounter++;
                                   }
                                   else if (SettingsModel.TimeMode == TimeMode.TimeFromComputer)
                                   {
                                       double.TryParse(dataIn[0], out double x);
                                       time = x / 1000;
                                   }

                                   double.TryParse(dataIn[i], out double value);

                                   xValues.Add(time);
                                   yValues.Add(value);


                                   if (saveDataPointActive)
                                   {
                                       DataPoint dataPoint = new DataPoint
                                       {
                                           VariableName = DataModels[i].SeriesName,
                                           X = time,
                                           Y = value
                                       };
                                       _ = FileManager.Instance.SaveDataPoint(dataPoint);
                                   }
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
                   IsConnected = false;
                   saveDataPointActive = false;

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
