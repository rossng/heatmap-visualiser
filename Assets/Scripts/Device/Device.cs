using UnityEngine;
using System.Collections;
using RestSharp;
using UnityEngine.UI;
using Zenject;

public class Device : MonoBehaviour
{
    public string DeviceId;
    public Text Text;
    private HeatmapApi _api;
    private DeviceInfo _deviceInfo;

    [Inject]
    public void Init(HeatmapApi api)
    {
        _api = api;
    }

    private void Start()
    {
        StartCoroutine(UpdateDeviceDetails());
    }

    private void Update()
    {
        Text.text = _deviceInfo.data[0].hardware_info;
    }

    private void OnDeviceInfoReceived(IRestResponse<DeviceInfo> response)
    {
        _deviceInfo = response.Data;
    }

    IEnumerator UpdateDeviceDetails()
    {
        while (true)
        {
            _api.RequestDeviceInfo(DeviceId, OnDeviceInfoReceived);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
