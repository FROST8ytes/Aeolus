namespace Lib.Configuration
{
    /// <summary>
    /// Application configuration model stored in %LocalAppData%\Aeolus.
    /// </summary>
    internal class AppConfig
    {
        internal int UpdateIntervalMs { get; set; } = 1000;
        internal bool RunOnStartup { get; set; } = false;
        internal bool StartMinimized { get; set; } = false;
        internal bool CloseToTray { get; set; } = false;
    }
}
