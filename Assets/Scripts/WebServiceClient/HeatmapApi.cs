using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestSharp;
using RestSharp.Deserializers;

public class HeatmapApi : MonoBehaviour
{
    static string _url = "http://34.251.68.107:5000";
    public static HeatmapApi Singleton;

    public HeatmapApi()
    {
        Debug.Log("Starting HeatmapApi");
        Debug.LogFormat("URL: {0}", _url);
    }

    public T Execute<T>(RestRequest request) where T : new()
    {
        var client = new RestClient {BaseUrl = new Uri(_url) };
        var response = client.Execute<T>(request);
        return response.Data;
    }

    public void ExecuteAsync<T>(RestRequest request, Action<IRestResponse<T>> callback) where T : new()
    {
        var client = new RestClient { BaseUrl = new Uri(_url) };
        client.ExecuteAsync(request, callback);
    }

    public void RequestDeviceInfo(string deviceId, Action<IRestResponse<DeviceInfo>> callback)
    {
        var request = new RestRequest
        {
            Resource = "api/reading/{device_id}",
            RootElement = "Reading"
        };
        request.AddParameter("device_id", deviceId, ParameterType.UrlSegment);
        ExecuteAsync(request, callback);
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
}
