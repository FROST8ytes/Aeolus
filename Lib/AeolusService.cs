using Lib.Configuration;
using Lib.Device;
using Lib.Device.Aeolus;
using Lib.Common;
using Serilog;

namespace Lib
{
    /// <summary>
    /// Main service that orchestrates hardware monitoring and device communication.
    /// </summary>
    public class AeolusService : IDisposable
    {
        private readonly ConfigManager _configManager;
        private readonly IDeviceFactory _deviceFactory;
        private readonly IDevice _device;
        private AppConfig _config;
        private Task? _updateTask;

        public AeolusService(string appName) : this(appName, new AeolusDeviceFactory())
        {
        }

        internal AeolusService(string appName, IDeviceFactory deviceFactory)
        {
            _configManager = new ConfigManager(appName);
            _config = _configManager.Load();
            _deviceFactory = deviceFactory;
            _device = _deviceFactory.CreateDevice();
            Logging.Initialize();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Starting AeolusService...");
            if (!_device.Connect())
            {
                Log.Error("Failed to connect to device.");
            }
            _updateTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int temperature = 99;
                    int rpm = 9900;
                    var packet = _deviceFactory.CreatePacket()
                        .SetTemperature(temperature)
                        .SetRpm(rpm);
                    _device.SendPacket(packet);
                    await Task.Delay(_config.UpdateIntervalMs, cancellationToken);
                }
            }, cancellationToken);
        }


        public void Dispose()
        {
            _device.Dispose();
            Logging.Dispose();
        }
    }
}
