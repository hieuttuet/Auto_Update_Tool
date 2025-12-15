using System.Windows;

namespace Updater
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Không có args → thoát
            if (e.Args == null || e.Args.Length == 0)
            {
                Shutdown();
                return;
            }

            var window = new MainWindow(e.Args);
            window.Show();
        }
    }
}
