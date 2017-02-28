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
    public string hardware_id { get; set; }
    public List<SensorInfo> sensor_info { get; set; }
}

public class DeviceInfo
{
    public List<Reading> data { get; set; }
    public string device_id { get; set; }
    public string id { get; set; }
    public long time_stamp { get; set; }
}

public class DeviceId
{
    public string device_id { get; set; }
}