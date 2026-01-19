using System.Text.Json;
using Microsoft.Win32;
using Serilog;

namespace Lib.Configuration
{
    /// <summary>
    /// Manages loading and saving application configuration to %LocalAppData%\Aeolus.
    /// </summary>
    internal class ConfigManager
    {
        private readonly string _configPath;
        private readonly string _appName;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        internal ConfigManager(string appName)
        {
            _appName = appName;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDirectory = Path.Combine(localAppData, _appName);
            Directory.CreateDirectory(appDirectory);
            _configPath = Path.Combine(appDirectory, "config.json");
        }

        internal AppConfig Load()
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

        internal void Save(AppConfig config)
        {
            Log.Debug("Saving configuration to {ConfigPath}", _configPath);
            var json = JsonSerializer.Serialize(config, _jsonOptions);
            File.WriteAllText(_configPath, json);
        }

        internal void SetRunOnStartup(bool enable, string executablePath, string arguments = "")
        {
            Log.Debug("Setting run on startup to {Enable} for {ExecutablePath} with arguments: {Arguments}", enable, executablePath, arguments);
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key == null) return;

            var valueName = $"Aeolus.{_appName}";

            if (enable)
            {
                var command = string.IsNullOrEmpty(arguments)
                    ? $"\"{executablePath}\""
                    : $"\"{executablePath}\" {arguments}";
                key.SetValue(valueName, command);
            }
            else
            {
                key.DeleteValue(valueName, false);
            }
        }
    }
}
