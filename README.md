# Aeolus Windows

A lightweight, background service and GUI application for monitoring and controlling the Invasion Aeolus 50 Pro CPU tower cooler.

## About

This is a reverse engineering project to make the 2x2 7-segment display on the cooler usable without using the sketchy program provided by the company.
The program provided by the company is a closed-source, limited in settings, ugly (in my opinion), and has little to no docs online.
It is also written using Vue, which is slow and inefficient.
This project aims to provide a safe, efficient, open-source alternative that respects user privacy and security.

## Features

- **Hardware Monitoring**: Reads CPU temperature and fan RPM using LibreHardwareMonitor.
- **Device Communication**: Communicates with the Invasion Aeolus 50 Pro (so far) via HID to update the built-in display.
- **System Tray Integration**: Runs quietly in the background with a system tray icon.
- **Configurable**: 
  - Adjustable update intervals
  - Start minimized to tray
  - Close to tray
  - Run on Windows startup (WIP)

## Architecture

The project is split into two main components:

1. **Lib**: The core library handling hardware monitoring (`LibreHardwareMonitor`), device communication (`HidSharp`), and configuration management.
2. **GUI**: A modern WPF application using `WPF-UI` for the user interface and system tray management.

## Requirements

- Windows 10/11 (x64)
- .NET 10.0 Runtime (tested)

## Installation

No binary download is included yet as this is an early-stage project. Plus, I am unable to sign the executable.

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

This project is licensed under MIT open-source. See the LICENSE file for details.
