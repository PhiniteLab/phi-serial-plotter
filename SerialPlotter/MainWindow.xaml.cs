using System.Windows;


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
