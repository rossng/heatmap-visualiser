using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UI;
using Zenject;

public class Gui : MonoBehaviour
{
    public GameObject IndicatorPrefab;
    public GameObject CurrentIndicator;
    public GameObject DevicePrefab;
    public ExtendedFlycam ExtendedFlycam;
    public List<Device> Devices;
    public DeviceList DeviceDropdown;
    public Button AddDeviceButton;

    public void AddDevice()
    {
        var dropdown = DeviceDropdown.GetComponent<Dropdown>();
        if (dropdown.value >= dropdown.options.Count) return;
        var deviceId = dropdown.options[dropdown.value].text;
        var obj = Instantiate(DevicePrefab, CurrentIndicator.transform.position, Quaternion.identity);
        var device = obj.GetComponent<Device>();
        Devices.Add(device);
        device.DeviceId = deviceId;
        ExtendedFlycam.UnlockCameraPosition();
        DeviceDropdown.SetInteractable(false);
        AddDeviceButton.interactable = false;
    }

    private void Start()
    {
        CurrentIndicator = Instantiate(IndicatorPrefab, Vector3.zero, Quaternion.identity);
        ExtendedFlycam = Camera.main.GetComponent<ExtendedFlycam>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Destroy(CurrentIndicator);
                CurrentIndicator = Instantiate(IndicatorPrefab, hit.point, Quaternion.identity);
                ExtendedFlycam.LockCameraPosition();
                DeviceDropdown.SetInteractable(true);
                AddDeviceButton.interactable = true;
            }
        } else if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var device = hit.transform.GetComponentInParent<Device>();
                if (device != null)
                {
                    Destroy(device.gameObject);
                    Devices.RemoveAll(d => d.DeviceId == device.DeviceId);
                }
            }
        }
    }
}
