using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using UnityEngine.UI;
using Zenject;

public class Gui : MonoBehaviour
{
    public static Gui Singleton;
    public GameObject IndicatorPrefab;
    public GameObject CurrentIndicator;
    public GameObject DevicePrefab;
    public ExtendedFlycam ExtendedFlycam;
    public List<Device> Devices;
    public DeviceList DeviceDropdown;
    public Button AddDeviceButton;
    public Text DateTimeText;
    public Scrollbar TimeScrollbar;
    public long RenderTimestamp = long.MaxValue;
    public long OldestTimestamp = long.MaxValue;
    public long NewestTimestamp = long.MinValue;

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

    public void OnTimeScrollbarValueChanged(Scrollbar scrollbar)
    {
        RenderTimestamp = (long)(OldestTimestamp + scrollbar.value * (NewestTimestamp - OldestTimestamp));
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        DateTime currentValue = epoch.AddMilliseconds(RenderTimestamp);
        DateTimeText.text = currentValue.ToString(CultureInfo.InvariantCulture);
    }

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
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
        if (Devices.Count > 0)
        {
            OldestTimestamp = Devices.OrderBy(d => d.OldestTimestamp).First().OldestTimestamp;
            NewestTimestamp = Devices.OrderByDescending(d => d.NewestTimestamp).First().NewestTimestamp;
            RenderTimestamp = (long)(OldestTimestamp + TimeScrollbar.value * (NewestTimestamp - OldestTimestamp));
            RenderTimestamp = Math.Max(RenderTimestamp, 0);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime currentValue = epoch.AddMilliseconds(RenderTimestamp);
            DateTimeText.text = currentValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
