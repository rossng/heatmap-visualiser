﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;

public class AddDeviceGui : MonoBehaviour
{
    private string _deviceId = "";
    public GameObject IndicatorPrefab;
    public GameObject CurrentIndicator;
    public GameObject DevicePrefab;
    public ExtendedFlycam ExtendedFlycam;
    public List<Device> Devices;

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 90), "Add Device");

        _deviceId = GUI.TextField(new Rect(25, 40, 250, 30), _deviceId);

        if (GUI.Button(new Rect(25, 75, 250, 20), "Add"))
        {
            var gameObject = Instantiate(DevicePrefab, CurrentIndicator.transform.position, Quaternion.identity);
            var device = gameObject.GetComponent<Device>();
            Devices.Add(device);
            device.DeviceId = _deviceId;
            _deviceId = "";
            ExtendedFlycam.UnlockCameraPosition();
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
            }
        }
    }
}
