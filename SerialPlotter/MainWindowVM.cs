using GalaSoft.MvvmLight.CommandWpf;
using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using SerialPlotter.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;


namespace SerialPlotter
{
    public class MainWindowVM
    {

        // Commands
        public ICommand connectButtonCommand { get; private set; }
        public ICommand closeConnectionButtonCommand { get; private set; }
        public ICommand createDataModelCommand { get; private set; }


        // Properties
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public List<int> BaudRateList { get; set; }
        public List<string> ComPortList { get; set; }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> MultiController { get; set; }



        // Data Models Configs.

        public int DataModelsCount { get; set; }
        public string DataModelSeperator { get; set; }
       
        public ObservableCollection<DataModel> DataModels { get; set; }

        public MainWindowVM()
        {
            connectButtonCommand = new RelayCommand(ConnectSerialPort);
            closeConnectionButtonCommand = new RelayCommand(CloseConnection);
            createDataModelCommand = new RelayCommand(CreateDataModels);

            GetSavedDataModels();

           
             BaudRateList = new List<int> { 9600, 115200 };
            ComPortList = SerialPort.GetPortNames().ToList();


            MultiController = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            MultiController.Range.MinimumY = 0;
            MultiController.Range.MaximumY = 1080;
            MultiController.Range.MaximumX = 10;
            MultiController.Range.AutoY = true;


        }


        private void GetSavedDataModels()
        {
            DataModels = DataModelProvider.Instance.DataModels;
            DataModelsCount = DataModels.Count;

        }


        private void CreateDataModels()
        {
            for (int i = 0; i < DataModelsCount; i++)
            {
                DataModel dataModel = new DataModel();
                dataModel.SeriesName = "Series Name " + i;
                DataModelProvider.Instance.DataModels.Add(dataModel);
                //AllColors = from PropertyInfo property in typeof(Colors).GetProperties() orderby property.Name select new ColorInfo(property.Name, (Color)property.GetValue(null, null));

                //CreateWpfGraphDataSeries(dataModel);
            }
            DataModelProvider.Instance.SaveDataModels();
        }

        public WpfGraphDataSeries CreateWpfGraphDataSeries(DataModel dataModel)
        {
            return new WpfGraphDataSeries()
            {
                Name = dataModel.SeriesName,
                Stroke = dataModel.SeriesColorInfo.Color,
            };
        }


        public void CloseConnection()
        {
           

            if (SerialConnection.IsConnected())
                SerialConnection.CloseConnection();

        }
        double counter = 0;
        public void ConnectSerialPort()
        {
            SerialConnection.SerialPortName = PortName;
            SerialConnection.SerialBaudRate = BaudRate;

            if (!SerialConnection.IsConnected())
            {

                SerialConnection.CreateConnection();
                Console.WriteLine("Connected");

                if (SerialConnection.IsConnected())
                {
                    MultiController.Clear();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            while (SerialConnection.SerialPort != null)
                            {

                                string data = SerialConnection.SerialPort.ReadLine().Replace(".", ",");
                                string[] dataIn = data.Replace('\n', '\0').Split(' ');

                                if (dataIn.Length > 1)
                                {
                                    List<DoubleDataPoint> yValues = new List<DoubleDataPoint>();
                                    List<DoubleDataPoint> xValues = new List<DoubleDataPoint>();

                                    // var x = watch.Elapsed;
                                    for (int i = 0; i < dataIn.Length - 1; i++)
                                    {
                                        // double.TryParse(dataIn[0], out double x);
                                        xValues.Add(counter);
                                        double.TryParse(dataIn[i], out double value);
                                        yValues.Add(value);
                                        counter++;

                                    }
                                    MultiController.PushData(xValues, yValues);
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message);
                        }
                    });
                }
            }

        }



    }
}
