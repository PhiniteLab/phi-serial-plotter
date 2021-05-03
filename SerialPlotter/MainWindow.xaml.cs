using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SerialPlotter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string selecetedPortName = "";
        List<string> ports;

        private ObservableCollection<DataPoint> points;

        public ObservableCollection<DataPoint> Points
        {
            get { return points; }
            set { points = value; }
        }



        int counter = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
            getPorts();

        }


        private void task()
        {

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 100;
            aTimer.Enabled = true;

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {


        }

        double firstTime = 0;
        bool controlTerm = false;
        private void connectToPort()
        {
            selecetedPortName = portNames.Text;
            SerialConnection.SerialPortName = selecetedPortName;
            SerialConnection.CreateConnection();

            //string[] dataIn = { "0" };

            //while (dataIn.Length < 2)
            //{
            //    string data = SerialConnection.ReadLine().Replace(".", ",");

            //    dataIn = data.Replace('\n', '\0').Split(' ');
            //    if (dataIn.Length > 1)

            //        if (double.TryParse(dataIn[0], out double time) && double.TryParse(dataIn[1], out double point))
            //        {
            //            firstTime = time;
            //            Console.WriteLine(firstTime);

            //            controlTerm = true;

            //        }
            //}

            //SerialConnection.SerialPort.DataReceived += SerialPort_DataReceived;


            points = new ObservableCollection<DataPoint>();
           
            Thread thread = new Thread(new ThreadStart(getSerialData));
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = true;
            thread.Start();
            // task();


        }

        double time;
        double point;
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {


            // getSerialData();

        }

        DataPoint dataPoint = new DataPoint();
        private void getSerialData()
        {

            while (true)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        string data = SerialConnection.SerialPort.ReadLine().Replace(".", ",");
                        string[] dataIn = data.Replace('\n', '\0').Split(' ');


                        if (dataIn.Length > 1)

                            if (double.TryParse(dataIn[0], out time) && double.TryParse(dataIn[1], out point))
                            {
                                //if (counter % (10000) == 0) { Clear(points); counter = 0; }
                                
                                dataPoint = new DataPoint(time, point);
                                points.Add(dataPoint);
                                
                                //counter++;

                                Console.WriteLine(time + " " + point);
                            }
                    });
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

            }


        }


        void Clear<T>(ObservableCollection<T> list)
        {
            list.Clear();
        }

        private void getPorts()
        {
            ports = SerialPort.GetPortNames().ToList();
            portNames.ItemsSource = ports;
            portNames.SelectedIndex = 0;
        }

        private void portNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            connectToPort();

        }

        private void recieveData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
