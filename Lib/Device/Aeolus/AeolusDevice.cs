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

        public bool DebugLogPackets { get; set; }

        public bool Connect()
        {
            Log.Information("Attempting to connect to Aeolus device (VID: {VendorId:X4}, PID: {ProductId:X4})", VendorId, ProductId);
            try
            {
                var deviceList = DeviceList.Local;
                _device = deviceList.GetHidDevices(VendorId, ProductId).FirstOrDefault();

                if (_device == null)
                {
                    Log.Warning("No device found with VID: {VendorId:X4}, PID: {ProductId:X4}", VendorId, ProductId);
                    return false;
                }

                Log.Information("Found device: {DevicePath}", _device.DevicePath);
                Log.Information("Max report lengths - Input: {MaxInput}, Output: {MaxOutput}, Feature: {MaxFeature}",
                    _device.GetMaxInputReportLength(),
                    _device.GetMaxOutputReportLength(),
                    _device.GetMaxFeatureReportLength());

                if (!_device.TryOpen(out _stream))
                {
                    Log.Warning("Unable to open device with VID: {VendorId:X4}", VendorId);
                    _device = null;
                    return false;
                }

                Log.Information("Successfully connected to device");
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
            if (_stream == null || !_stream.CanWrite || _device == null)
            {
                return false;
            }
            try
            {
                var packetBytes = packet.ToBytes();
                var outputReportLength = _device.GetMaxOutputReportLength();

                // Create buffer matching actual output report size
                var buffer = new byte[outputReportLength];
                Array.Copy(packetBytes, buffer, Math.Min(packetBytes.Length, outputReportLength));

                if (DebugLogPackets)
                {
                    Log.Information("TX [{Length} bytes]: {PacketHex}",
                        buffer.Length,
                        string.Join(" ", buffer.Select(b => b.ToString("X2"))));
                }

                _stream.Write(buffer);
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
