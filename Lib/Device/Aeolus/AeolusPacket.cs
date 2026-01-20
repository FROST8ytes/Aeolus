using Serilog;

namespace Lib.Device.Aeolus
{
    /// <summary>
    /// Builds the 8-byte HID packet for the Invasion Aeolus 50 Pro CPU tower cooler.
    /// </summary>
    internal class AeolusPacket : IPacket
    {
        private const byte ReportId = 0x02;
        private const byte Header = 0x40;
        private byte Temperature;
        private uint Rpm;

        internal AeolusPacket(int temperature = 0, int rpm = 0)
        {
            Log.Debug("Creating AeolusPacket with Temperature: {Temperature}, RPM: {Rpm}", temperature, rpm);
            Temperature = (byte)Math.Clamp(temperature, 0, 99);
            Rpm = (uint)Math.Clamp(rpm, 0, 9900);
        }

        public IPacket SetTemperature(int temperature)
        {
            Log.Debug("Setting Temperature to: {Temperature}", temperature);
            Temperature = (byte)Math.Clamp(temperature, 0, 99);
            return this;
        }

        public IPacket SetRpm(int rpm)
        {
            Log.Debug("Setting RPM to: {Rpm}", rpm);
            Rpm = (uint)Math.Clamp(rpm, 0, 9900);
            return this;
        }

        public byte[] ToBytes()
        {
            Log.Debug("Serializing AeolusPacket to bytes with Temperature: {Temperature}, RPM: {Rpm}", Temperature, Rpm);
            var rpmHighByte = (byte)(Rpm / 256);
            var rpmLowByte = (byte)(Rpm % 256);
            
            return [
                ReportId,
                Header,
                Temperature,
                0x00,
                0x00,
                0x00,
                rpmHighByte,
                rpmLowByte
            ];
        }
    }
}
