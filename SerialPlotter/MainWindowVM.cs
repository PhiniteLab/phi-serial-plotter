using GalaSoft.MvvmLight.CommandWpf;
using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
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


        // Properties
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public List<int> BaudRateList { get; set; }
        public List<string> ComPortList { get; set; }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> MultiController { get; set; }


        public MainWindowVM()
        {
            connectButtonCommand = new RelayCommand(ConnectSerialPort);
            closeConnectionButtonCommand = new RelayCommand(CloseConnection);


            BaudRateList = new List<int> { 9600, 115200 };
            ComPortList = SerialPort.GetPortNames().ToList();


            MultiController = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            MultiController.Range.MinimumY = 0;
            MultiController.Range.MaximumY = 1080;
            MultiController.Range.MaximumX = 10;
            MultiController.Range.AutoY = true;
           

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 1",
                Stroke = Colors.Red,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 2",
                Stroke = Colors.Green,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 3",
                Stroke = Colors.Blue,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 4",
                Stroke = Colors.Yellow,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 5",
                Stroke = Colors.Orange,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 6",
                Stroke = Colors.White,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 7",
                Stroke = Colors.Pink,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 8",
                Stroke = Colors.BlueViolet,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 9",
                Stroke = Colors.Brown,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 10",
                Stroke = Colors.DarkGoldenrod,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 11",
                Stroke = Colors.Silver,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 12",
                Stroke = Colors.AliceBlue,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 13",
                Stroke = Colors.YellowGreen,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 14",
                Stroke = Colors.Peru,
            });

            MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 15",
                Stroke = Colors.Crimson,
            });


        }



        public void CloseConnection()
        {
            if (SerialConnection.IsConnected())
                SerialConnection.CloseConnection();

        }

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
                        while (true)
                        {

                            string data = SerialConnection.SerialPort.ReadLine().Replace(".", ",");
                            string[] dataIn = data.Replace('\n', '\0').Split(' ');

                            if (dataIn.Length > 1)
                            {
                                List<DoubleDataPoint> yValues = new List<DoubleDataPoint>();
                                List<DoubleDataPoint> xValues = new List<DoubleDataPoint>();

                                //var x = watch.Elapsed;
                                for (int i = 0; i < dataIn.Length - 1; i++)
                                {
                                    double.TryParse(dataIn[0], out double x);
                                    xValues.Add(x / 1000);
                                    double.TryParse(dataIn[i + 1], out double value);
                                    yValues.Add(value);
                                    

                                }
                                MultiController.PushData(xValues, yValues);
                            }
                            // Thread.Sleep(1);
                        }
                    });
                }
            }

        }
    }
}
