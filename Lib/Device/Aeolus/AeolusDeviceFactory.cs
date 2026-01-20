namespace Lib.Device.Aeolus
{
    /// <summary>
    /// Factory for creating Aeolus device and packet instances.
    /// </summary>
    internal class AeolusDeviceFactory : IDeviceFactory
    {
        public IDevice CreateDevice() => new AeolusDevice();

        public IPacket CreatePacket(int temperature = 0, int rpm = 0) => new AeolusPacket(temperature, rpm);
    }
}
