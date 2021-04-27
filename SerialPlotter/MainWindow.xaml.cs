﻿using OxyPlot;
using OxyPlot.Series;
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

        private IList<DataPoint> points;

        public IList<DataPoint> Points
        {
            get { return points; }
            set { points = value; }
        }



         long counter = 0;
        public MainWindow()
        {
            InitializeComponent();
            getPorts();

        }


        private void task()
        {

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 10;
            aTimer.Enabled = true;

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    double point = double.Parse(SerialConnection.ReadLine());
                    points.Add(new DataPoint(counter, point));
                    lineSeries.ItemsSource = points;
                    counter++;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            });
        }
        private void connectToPort()
        {
            selecetedPortName = portNames.Text;
            SerialConnection.SerialPortName = selecetedPortName;
            SerialConnection.CreateConnection();
            points = new List<DataPoint>();
            task();


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
