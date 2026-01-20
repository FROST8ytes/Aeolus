namespace Lib.Device
{
    /// <summary>
    /// Abstract factory interface for creating device and packet instances.
    /// Implementations create compatible device-packet combinations.
    /// </summary>
    internal interface IDeviceFactory
    {
        IDevice CreateDevice();
        IPacket CreatePacket(int temperature = 0, int rpm = 0);
    }
}
