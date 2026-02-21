# Aeolus

A lightweight, background service and GUI application for monitoring and controlling the Invasion Aeolus 50 Pro CPU tower cooler.

## Features

- **Hardware Monitoring**: Reads CPU temperature and fan RPM using LibreHardwareMonitor.
- **Device Communication**: Communicates with the Aeolus cooler via HID to update the built-in display.
- **System Tray Integration**: Runs quietly in the background with a system tray icon.
- **Configurable**: 
  - Adjustable update intervals
  - Start minimized to tray
  - Close to tray
  - Run on Windows startup
- **Debugging**: Built-in packet capture (PCAPNG) and hex logging for troubleshooting device communication.

## Architecture

The project is split into two main components:

1. **Lib**: The core library handling hardware monitoring (`LibreHardwareMonitor`), device communication (`HidSharp`), and configuration management.
2. **GUI**: A modern WPF application using `WPF-UI` for the user interface and system tray management.

## Requirements

- Windows 10/11 (x64)
- .NET 10.0 Runtime

## Building from Source

1. Clone the repository:
   ```bash
   git clone https://github.com/FROST8ytes/Aeolus.git
   ```
2. Open the solution in Visual Studio 2022 or use the .NET CLI:
   ```bash
   dotnet build
   ```

## Configuration

Configuration is stored in `%LocalAppData%\Aeolus\config.json`. 

### Debugging

If you are experiencing issues with device communication, you can enable debug logging by editing the `config.json` file:

```json
{
  "DebugLogPackets": true,
  "DebugPacketCapture": true
}
```

- `DebugLogPackets`: Logs the raw hex bytes sent to the device in the application logs.
- `DebugPacketCapture`: Generates a `.pcapng` file in the `%LocalAppData%\Aeolus` directory containing the raw USB HID packets, which can be analyzed in Wireshark.

Logs are stored in `%LocalAppData%\Aeolus\Logs`.

## License

This project is open-source. See the LICENSE file for details.
