using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SensorReading
{
    public string tag { get; set; }
    public float value { get; set; }
}

public class Hardware
{
    public string hardware_id { get; set; }
    public List<SensorReading> sensor_info { get; set; }
}

public class DeviceData
{
    public List<Hardware> data { get; set; }
    public string device_id { get; set; }
    public string id { get; set; }
    public long time_stamp { get; set; }
}

public class DeviceHistory
{
    public List<DeviceData> data { get; set; }
}

public class DeviceId
{
    public string device_id { get; set; }
}