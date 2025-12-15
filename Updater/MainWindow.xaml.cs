using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    public partial class MainWindow : Window
    {
        private readonly string[] _args;

        public MainWindow(string[] args)
        {
            InitializeComponent();
            _args = args;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                UpdaterCore.Run(_args);
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}
