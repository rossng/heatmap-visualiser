using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SensorInfo
{
    public string tag { get; set; }
    public float value { get; set; }
}

public class Reading
{
    public long time { get; set; }
    public string hardware_info { get; set; }
    public List<SensorInfo> sensor_info { get; set; }
}

public class DeviceInfo
{
    public List<Reading> data { get; set; }
}