using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DeviceInfo
{
    public string tag;
    public float value;
}

public class Reading
{
    public long time { get; set; }
    public DeviceInfo device_info { get; set; }
}
