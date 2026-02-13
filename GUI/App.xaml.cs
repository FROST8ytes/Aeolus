using System.Windows;
using Lib;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string AppName = "Aeolus";
        private AeolusService? _service;
        private CancellationTokenSource? _cts;

        public AeolusService Service => _service!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _service = new AeolusService(AppName);
            _cts = new CancellationTokenSource();

            await _service.StartAsync(_cts.Token);

            MainWindow = new MainWindow();

            if (_service.ConfigManager.Config.StartMinimized)
            {
                // Show and immediately hide to initialize the visual tree (including tray icon)
                MainWindow.ShowInTaskbar = false;
                MainWindow.Show();
                MainWindow.Hide();
            }
            else
            {
                MainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _cts?.Cancel();
            _service?.Dispose();
            base.OnExit(e);
        }
    }
}
