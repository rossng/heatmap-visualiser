using System;
using UnityEngine;
using System.Collections;
using RestSharp;
using UnityEngine.UI;
using Zenject;

public class Display : MonoBehaviour
{
    private Text _text;
    private HeatmapApi _api;

    [Inject]
    public void Init(HeatmapApi api)
    {
        _api = api;
    }

    public void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void OnDeviceInfoReceived(IRestResponse<DeviceInfo> deviceInfo)
    {
        _text.text = deviceInfo.Data.data[0].hardware_info;
    }

    public void Start()
    {
        //_api.RequestDeviceInfo("19e7be15-9aa4-42c9-9a0e-d72407ec18bc", OnDeviceInfoReceived);
    }
}
