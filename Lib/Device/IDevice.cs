namespace Lib.Device
{
    /// <summary>
    /// Interface for hardware devices that can send packets.
    /// </summary>
    internal interface IDevice : IDisposable
    {
        bool Connect();
        bool SendPacket(IByteSerializable packet);
    }
}
