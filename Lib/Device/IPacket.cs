namespace Lib.Device
{
    /// <summary>
    /// Interface for device packets that can be configured with temperature and RPM.
    /// </summary>
    internal interface IPacket : IByteSerializable
    {
        IPacket SetTemperature(int temperature);
        IPacket SetRpm(int rpm);
    }
}
