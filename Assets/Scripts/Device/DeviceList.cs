using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;

public class DeviceList : MonoBehaviour
{
    private HeatmapApi _api;
    private Dropdown _dropdown;
    private List<DeviceId> _devices;
    public Gui Gui;

    private void Awake()
    {
        _dropdown = GetComponent<Dropdown>();
    }

    private void Start()
    {
        _api = HeatmapApi.Singleton;
        StartCoroutine(UpdateDeviceList());
    }

    private void OnDeviceListResponse(IRestResponse<List<DeviceId>> devices)
    {
        _devices = devices.Data;
    }

    private IEnumerator UpdateDeviceList()
    {
        while (true)
        {
            _api.RequestDeviceList(OnDeviceListResponse);
            yield return new WaitForSeconds(30.0f);
        }
    }

    private void Update()
    {
        if (_devices == null) return;
        _dropdown.options = _devices
            .Select(d => new Dropdown.OptionData(d.device_id))
            .Where(d => Gui.Devices.All(e => e.DeviceId != d.text))
            .ToList();
    }

    public void SetInteractable(bool to)
    {
        _dropdown.interactable = to;
    }
}
