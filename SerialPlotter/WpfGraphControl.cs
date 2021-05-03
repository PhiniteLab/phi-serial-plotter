using RealTimeGraphX;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SerialPlotter
{
    public class WpfGraphControl : Control
    {
        /// <summary>
        /// Gets or sets the graph controller.
        /// </summary>
        public IGraphController Controller
        {
            get { return (IGraphController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IGraphController), typeof(WpfGraphControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether to display a tool tip with the current cursor value.
        /// </summary>
        public bool DisplayToolTip
        {
            get { return (bool)GetValue(DisplayToolTipProperty); }
            set { SetValue(DisplayToolTipProperty, value); }
        }
        public static readonly DependencyProperty DisplayToolTipProperty =
            DependencyProperty.Register("DisplayToolTip", typeof(bool), typeof(WpfGraphControl), new PropertyMetadata(false));


        /// <summary>
        /// Initializes the <see cref="WpfGraphControl"/> class.
        /// </summary>
        static WpfGraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfGraphControl), new FrameworkPropertyMetadata(typeof(WpfGraphControl)));
            
        }
    }
}
