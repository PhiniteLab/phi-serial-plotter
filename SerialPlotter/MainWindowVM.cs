using RealTimeGraphX;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SerialPlotter
{
    public class MainWindowVM
    {
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        //public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> MultiController { get; set; }

        public MainWindowVM()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1080;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller.Range.AutoY = true;
            Controller.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series",
                Stroke = Colors.DodgerBlue,
            });

            //MultiController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            //MultiController.Range.MinimumY = 0;
            //MultiController.Range.MaximumY = 1080;
            //MultiController.Range.MaximumX = TimeSpan.FromSeconds(10);
            //MultiController.Range.AutoY = true;

            //MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            //{
            //    Name = "Series 1",
            //    Stroke = Colors.Red,
            //});

            //MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            //{
            //    Name = "Series 2",
            //    Stroke = Colors.Green,
            //});

            //MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            //{
            //    Name = "Series 3",
            //    Stroke = Colors.Blue,
            //});

            //MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            //{
            //    Name = "Series 4",
            //    Stroke = Colors.Yellow,
            //});

            //MultiController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            //{
            //    Name = "Series 5",
            //    Stroke = Colors.Gray,
            //});

            Stopwatch watch = new Stopwatch();
            watch.Start();


            SerialConnection.SerialPortName = "COM8";
            SerialConnection.CreateConnection();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    string data = SerialConnection.SerialPort.ReadLine().Replace(".", ",");
                    string[] dataIn = data.Replace('\n', '\0').Split(' ');


                    if (dataIn.Length > 1)

                        if (double.TryParse(dataIn[0], out double time) && double.TryParse(dataIn[1], out double point))
                        {
                            var x = watch.Elapsed;
                            Controller.PushData(x, point);

                        }


                   // Thread.Sleep(1);
                }
            });
        }
    }
}
