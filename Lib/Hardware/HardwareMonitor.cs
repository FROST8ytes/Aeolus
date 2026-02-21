using LibreHardwareMonitor.Hardware;
using Serilog;

namespace Lib.Hardware
{
    internal class HardwareMonitor : IDisposable
    {
        private readonly Computer _computer;

        public HardwareMonitor()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true
            };

            try
            {
                _computer.Open();
                _computer.Accept(new UpdateVisitor());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to open LibreHardwareMonitor computer");
            }
        }

        private class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware)
                {
                    subHardware.Accept(this);
                }
            }

            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        public (int temperature, int rpm) GetReadings()
        {
            int temp = 0;
            int rpm = 0;

            try
            {
                _computer.Accept(new UpdateVisitor());

                foreach (var hardware in _computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Log.Debug("Found CPU Temp Sensor: {Name} = {Value}", sensor.Name, sensor.Value);

                                if (sensor.Name.Contains("Package", StringComparison.OrdinalIgnoreCase) || 
                                    sensor.Name.Contains("Core Average", StringComparison.OrdinalIgnoreCase) ||
                                    sensor.Name.Contains("Tctl", StringComparison.OrdinalIgnoreCase) ||
                                    sensor.Name.Contains("Tdie", StringComparison.OrdinalIgnoreCase) ||
                                    sensor.Name.Contains("Core (Tctl/Tdie)", StringComparison.OrdinalIgnoreCase))
                                {
                                    temp = Math.Max(temp, (int)(sensor.Value ?? 0));
                                }
                            }
                        }

                        if (temp == 0)
                        {
                            foreach (var sensor in hardware.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core", StringComparison.OrdinalIgnoreCase))
                                {
                                    temp = Math.Max(temp, (int)(sensor.Value ?? 0));
                                }
                            }
                        }
                    }

                    if (hardware.HardwareType == HardwareType.Motherboard)
                    {
                        foreach (var sub in hardware.SubHardware)
                        {
                            foreach (var sensor in sub.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Fan)
                                {
                                    rpm = Math.Max(rpm, (int)(sensor.Value ?? 0));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Fan)
                            {
                                rpm = Math.Max(rpm, (int)(sensor.Value ?? 0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error reading hardware sensors");
            }

            return (temp, rpm);
        }

        public void Dispose()
        {
            try
            {
                _computer.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error closing LibreHardwareMonitor computer");
            }
        }
    }
}