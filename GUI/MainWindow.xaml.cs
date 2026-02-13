using System.ComponentModel;
using System.Windows;
using Lib;
using Lib.Configuration;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ConfigManager _configManager;
        private bool _isInitialized;

        public MainWindow()
        {
            InitializeComponent();
            _configManager = ((App)Application.Current).Service.ConfigManager;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateIntervalInput.Value = _configManager.Config.UpdateIntervalMs;
            CloseToTrayToggle.IsChecked = _configManager.Config.CloseToTray;
            StartMinimizedToggle.IsChecked = _configManager.Config.StartMinimized;
            RunAtStartupToggle.IsChecked = _configManager.Config.RunOnStartup;
            _isInitialized = true;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_configManager.Config.CloseToTray)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void UpdateIntervalInput_ValueChanged(object sender, RoutedEventArgs args)
        {
            if (!_isInitialized || UpdateIntervalInput.Value is null)
            {
                return;
            }

            _configManager.Update(c => c.UpdateIntervalMs = (int)UpdateIntervalInput.Value);
        }

        private void CloseToTrayToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            _configManager.Update(c => c.CloseToTray = CloseToTrayToggle.IsChecked == true);
        }

        private void StartMinimizedToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            _configManager.Update(c => c.StartMinimized = StartMinimizedToggle.IsChecked == true);
        }

        private void RunAtStartupToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                return;
            }

            _configManager.Update(c => c.RunOnStartup = RunAtStartupToggle.IsChecked == true);
        }

        private void TrayIcon_LeftClick(Wpf.Ui.Tray.Controls.NotifyIcon sender, RoutedEventArgs e)
        {
            Show();
            base.WindowState = WindowState.Normal;
            Activate();
        }

        private void TrayMenu_Show_Click(object sender, RoutedEventArgs e)
        {
            Show();
            base.WindowState = WindowState.Normal;
            Activate();
        }

        private void TrayMenu_Exit_Click(object sender, RoutedEventArgs e)
        {
            TrayIcon.Dispose();
            Application.Current.Shutdown();
        }
    }
}