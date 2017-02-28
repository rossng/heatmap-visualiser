using UnityEngine;
using System.Collections;
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
    private DeviceInfo _deviceInfo;

    private void Start()
    {
        _api = HeatmapApi.Singleton;
        StartCoroutine(UpdateDeviceDetails());
    }

    private void Update()
    {
        if (_deviceInfo == null) return;

        // Set device colour
        var sensorReading = _deviceInfo.data
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

    private void OnDeviceInfoReceived(IRestResponse<DeviceInfo> response)
    {
        Debug.LogFormat("Received response for device {0}", DeviceId);
        _deviceInfo = response.Data;
    }

    IEnumerator UpdateDeviceDetails()
    {
        while (true)
        {
            _api.RequestDeviceInfo(DeviceId, OnDeviceInfoReceived);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
