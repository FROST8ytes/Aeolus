using System.Text.Json;
using Microsoft.Win32;
using Serilog;

namespace Lib.Configuration
{
    /// <summary>
    /// Manages loading and saving application configuration to %LocalAppData%\Aeolus.
    /// </summary>
    public class ConfigManager
    {
        private readonly string _configPath;
        private readonly string _appName;
        private readonly string _executablePath;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public AppConfig Config { get; private set; }

        public ConfigManager(string appName)
        {
            _appName = appName;
            _executablePath = Environment.ProcessPath ?? string.Empty;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDirectory = Path.Combine(localAppData, _appName);
            Directory.CreateDirectory(appDirectory);
            _configPath = Path.Combine(appDirectory, "config.json");
            Config = Load();
        }

        /// <summary>
        /// Updates configuration properties and handles any side effects.
        /// </summary>
        /// <param name="modifier">Action to modify configuration properties.</param>
        public void Update(Action<AppConfig> modifier)
        {
            var previousRunOnStartup = Config.RunOnStartup;

            modifier(Config);

            // Handle side effects for properties that need them
            if (Config.RunOnStartup != previousRunOnStartup)
            {
                ApplyRunOnStartup(Config.RunOnStartup);
            }

            Save();
        }

        private AppConfig Load()
        {
            Log.Debug("Loading configuration from {ConfigPath}", _configPath);
            if (!File.Exists(_configPath))
            {
                Log.Information("Configuration file not found. Creating default configuration.");
                return new AppConfig();
            }
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? new AppConfig();
        }

        private void Save()
        {
            Log.Debug("Saving configuration to {ConfigPath}", _configPath);
            var json = JsonSerializer.Serialize(Config, _jsonOptions);
            File.WriteAllText(_configPath, json);
        }

        private void ApplyRunOnStartup(bool enable)
        {
            Log.Debug("Setting run on startup to {Enable} for {ExecutablePath}", enable, _executablePath);
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key == null) return;

            var valueName = $"Aeolus.{_appName}";

            if (enable)
            {
                key.SetValue(valueName, $"\"{_executablePath}\"");
            }
            else
            {
                key.DeleteValue(valueName, false);
            }
        }
    }
}
