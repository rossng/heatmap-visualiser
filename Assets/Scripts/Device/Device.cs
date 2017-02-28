using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using UnityEngine.UI;
using Zenject;

public class Device : MonoBehaviour
{
    public string DeviceId;
    public Text Text;
    public Renderer MeshRenderer;
    private HeatmapApi _api;
    private Gui _gui;
    public readonly List<DeviceData> DeviceData = new List<DeviceData>();
    public long OldestTimestamp = long.MaxValue;
    public long NewestTimestamp = long.MinValue;

    private void Start()
    {
        OldestTimestamp = long.MaxValue;
        _api = HeatmapApi.Singleton;
        _gui = Gui.Singleton;
        _api.RequestReadingsFrom(DeviceId, (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds - 100000L, OnDeviceHistoryReceived);
        StartCoroutine(UpdateDeviceDetails());
    }

    private void Update()
    {
        if (DeviceData == null) return;
        var deviceData = DeviceData.ToList();
        // Set device colour
        if (deviceData.Count == 0) return;

        var readingsAfterTimestamp = deviceData.OrderBy(d => d.time_stamp).Where(d => d.time_stamp >= _gui.RenderTimestamp).ToList();
        DeviceData reading = readingsAfterTimestamp.FirstOrDefault();
        if (!readingsAfterTimestamp.Any())
        {
            reading = deviceData.OrderBy(d => d.time_stamp).Last();
        }

        var sensorReading = reading.data
            .First(d => d.hardware_id.Contains("cpu")).sensor_info
            .First(s => s.tag.Contains("temp"));

        var cpuTemperature = sensorReading.value;

        var cpuTemperatureLow = 20.0f;
        var cpuTemperatureHigh = 95.0f;
        var cpuTemperatureClamped = Mathf.Clamp(cpuTemperature, cpuTemperatureLow, cpuTemperatureHigh);

        var cpuTemperatureFraction = 0.3f - (0.3f * (cpuTemperatureClamped - cpuTemperatureLow) /
                                     (cpuTemperatureHigh - cpuTemperatureLow));

        var temperatureColour = Color.HSVToRGB(cpuTemperatureFraction, 1.0f, 1.0f);

        MeshRenderer.material.color = temperatureColour;

        // Set particle gradient
        var particleSystem = GetComponentInChildren<ParticleSystem>();
        GradientColorKey[] gradientColours = particleSystem.colorOverLifetime.color.gradient.colorKeys;
        if (temperatureColour != gradientColours[0].color)
        {
            var newGradient = new Gradient();
            newGradient.SetKeys(new GradientColorKey[]
            {
                new GradientColorKey(temperatureColour, gradientColours[0].time),
                new GradientColorKey(gradientColours[1].color, gradientColours[1].time)
            }, particleSystem.colorOverLifetime.color.gradient.alphaKeys);

            ParticleSystem.ColorOverLifetimeModule colourModule;
            colourModule = particleSystem.colorOverLifetime;
            colourModule.color = newGradient;
        }

        // Set device tooltip
        Text.text = string.Format("{0}: {1}", sensorReading.tag, sensorReading.value);
    }

    private void OnDeviceInfoReceived(IRestResponse<DeviceData> response)
    {
        Debug.LogFormat("Received response for device {0}", DeviceId);
        // Thread-safe? Not really...
        DeviceData.Add(response.Data);
        if (response.Data.time_stamp > NewestTimestamp)
        {
            NewestTimestamp = response.Data.time_stamp;
        }
    }

    private void OnDeviceHistoryReceived(IRestResponse<DeviceHistory> response)
    {
        Debug.LogFormat("Received history ({0} items) for device {1}", response.Data.data.Count, DeviceId);
        DeviceData.AddRange(response.Data.data);

        long oldestTimestamp = response.Data.data.OrderBy(d => d.time_stamp).First().time_stamp;
        if (oldestTimestamp < OldestTimestamp && oldestTimestamp > 0)
        {
            OldestTimestamp = oldestTimestamp;
        }
    }

    private IEnumerator UpdateDeviceDetails()
    {
        while (true)
        {
            _api.RequestDeviceInfo(DeviceId, OnDeviceInfoReceived);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
