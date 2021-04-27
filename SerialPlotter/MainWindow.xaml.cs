using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            getPorts();
            
        }


        private void task()
        {
            SerialConnection.StartReceivingData();
            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 10;
            //aTimer.Enabled = true;

        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    Console.WriteLine(SerialConnection.ReadLine());
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            });
        }
        private void connectToPort()
        {
            SerialConnection.SerialPortName = selecetedPortName;

            SerialConnection.CreateConnection();
            SerialConnection.SerialPort.DataReceived += SerialPort_DataReceived; 

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    Console.WriteLine(SerialConnection.ReadLine());
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            });

           
        }

        private void getPorts()
        {
            ports = SerialPort.GetPortNames().ToList();
            portNames.ItemsSource = ports;
            portNames.SelectedIndex = 0;
        }

        private void portNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selecetedPortName = portNames.Text;
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            connectToPort();
            
        }

        private void recieveData_Click(object sender, RoutedEventArgs e)
        {
           task();
        }
    }
}
