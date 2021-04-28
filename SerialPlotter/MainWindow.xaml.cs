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



        double counter = 0;
        public MainWindow()
        {
            InitializeComponent();
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
            counter++;
            //string[] dataIn = SerialConnection.ReadLine().Replace('\n', '\0').Replace('\r', '\0').Split(' ');
            string dataIn = SerialConnection.ReadLine().Replace(".", ",");
            double.TryParse(dataIn, out double point);
            Console.WriteLine(dataIn);

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {

                    points.Add(new DataPoint(counter, point));

                    //Console.WriteLine(time + " " + dataIn[1]);
                    //if (dataIn.Length > 1)

                    //    if (long.TryParse(dataIn[0], out long time) && double.TryParse(SerialConnection.ReadLine(), out double point))
                    //    {

                    //    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            });

        }

        double firstTime = 0;
        bool controlTerm = false;
        private void connectToPort()
        {
            selecetedPortName = portNames.Text;
            SerialConnection.SerialPortName = selecetedPortName;
            SerialConnection.CreateConnection();

            string[] dataIn = { "0" };

            while (dataIn.Length < 2)
            {
                string data = SerialConnection.ReadLine().Replace(".", ",");
               
                dataIn = data.Replace('\n', '\0').Split(' ');
                if (dataIn.Length > 1)

                    if (double.TryParse(dataIn[0], out double time) && double.TryParse(dataIn[1], out double point))
                    {
                        firstTime = time;
                        Console.WriteLine(firstTime);

                        controlTerm = true;

                    }
            }

            SerialConnection.SerialPort.DataReceived += SerialPort_DataReceived;


            points = new ObservableCollection<DataPoint>();
            lineSeries.ItemsSource = points;
            // task();


        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //counter++;

            // Console.WriteLine(dataIn);

            if (controlTerm == true)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    try
                    {
                        string data = SerialConnection.ReadLine().Replace(".", ",");
                        string[] dataIn = data.Replace('\n', '\0').Split(' ');
                        //Console.WriteLine(data);

                        if (dataIn.Length > 1)

                            if (double.TryParse(dataIn[0], out double time) && double.TryParse(dataIn[1], out double point))
                            {
                                double finalTime = time - firstTime;
                                points.Add(new DataPoint(finalTime, point));
                               
                            }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                });
            }
            else
            {
               // points.Clear();
            }


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
