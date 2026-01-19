using Lib.Configuration;
using Lib.Device;
using Serilog;

namespace Lib
{
    /// <summary>
    /// Main service that orchestrates hardware monitoring and device communication.
    /// </summary>
    public class AeolusService : IDisposable
    {
        private readonly ConfigManager _configManager;
        private readonly AeolusDevice _device;
        private AppConfig _config;
        private Task? _updateTask;

        public AeolusService(string appName)
        {
            _configManager = new ConfigManager(appName);
            _config = _configManager.Load();
            _device = new AeolusDevice();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Starting AeolusService...");
            if (!_device.Connect())
            {
                Log.Error("Failed to connect to Aeolus device.");
            }
            _updateTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int temperature = 99;
                    int rpm = 9900;
                    var packet = new AeolusPacket()
                        .SetTemperature(temperature)
                        .SetRpm(rpm);
                    _device.SendPacket(packet);
                    await Task.Delay(_config.UpdateIntervalMs, cancellationToken);
                }
            }, cancellationToken);
        }


        public void Dispose()
        {
            // Dispose resources here
        }
    }
}
