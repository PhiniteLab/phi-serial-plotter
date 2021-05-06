using System.Windows;
using System.Windows.Controls;

namespace SerialPlotter
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
        }

    }
}
