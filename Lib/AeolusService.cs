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
        private readonly IDeviceFactory _deviceFactory;
        private readonly IDevice _device;
        private readonly ConfigManager _configManager;
        private Task? _updateTask;

        public ConfigManager ConfigManager => _configManager;

        public AeolusService(string appName) : this(appName, new AeolusDeviceFactory())
        {
        }

        internal AeolusService(string appName, IDeviceFactory deviceFactory)
        {
            _configManager = new ConfigManager(appName);
            _deviceFactory = deviceFactory;
            _device = _deviceFactory.CreateDevice();
            _device.DebugLogPackets = _configManager.Config.DebugLogPackets;
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
                int temperature = 0;
                int rpm = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var packet = _deviceFactory.CreatePacket()
                        .SetTemperature(temperature)
                        .SetRpm(rpm);
                    _device.SendPacket(packet);

                    temperature = (temperature + 1) % 100;
                    rpm = (rpm + 100) % 10000;

                    await Task.Delay(_configManager.Config.UpdateIntervalMs, cancellationToken);
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
