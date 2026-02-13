namespace Lib.Configuration
{
    /// <summary>
    /// Application configuration model stored in %LocalAppData%\Aeolus.
    /// </summary>
    public class AppConfig
    {
        public int UpdateIntervalMs { get; set; } = 1000;
        public bool RunOnStartup { get; set; } = false;
        public bool StartMinimized { get; set; } = false;
        public bool CloseToTray { get; set; } = false;
        public bool DebugLogPackets { get; set; } = true;
    }
}
