using HidSharp;
using Serilog;

namespace Lib.Device.Aeolus
{
    /// <summary>
    /// HID device implementation for the Invasion Aeolus 50 Pro CPU tower cooler.
    /// </summary>
    internal class AeolusDevice : IDevice
    {
        private const int VendorId = 0x5131;
        private const int ProductId = 0x2007;

        private HidDevice? _device;
        private HidStream? _stream;

        public bool Connect()
        {
            Log.Information("Attempting to connect to Aeolus device (VID: {VendorId:X4}, PID: {ProductId:X4})", VendorId, ProductId);
            try
            {
                var deviceList = DeviceList.Local;
                _device = deviceList.GetHidDevices(VendorId, ProductId).FirstOrDefault();

                if (_device == null)
                {
                    Log.Warning($"No device found with vendor ID {VendorId} and product ID {ProductId}");
                    return false;
                }

                if (!_device.TryOpen(out _stream))
                {
                    Log.Warning($"Unable to open device with vendor ID {VendorId}");
                    _device = null;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while connecting to Aeolus device");
                return false;
            }
        }

        public bool SendPacket(IByteSerializable packet)
        {
            if (_stream == null || !_stream.CanWrite)
            {
                return false;
            }
            try
            {
                Log.Information("Sending packet to Aeolus device");
                _stream.Write(packet.ToBytes());
                return true;
            }
            catch (Exception ex)
            {
                Dispose();
                Log.Error(ex, "Exception occurred while sending packet to Aeolus device");
                return false;
            }
        }

        public void Dispose()
        {
            Log.Debug("Disposing AeolusDevice resources");
            _stream?.Dispose();
            _stream = null;
        }
    }
}
