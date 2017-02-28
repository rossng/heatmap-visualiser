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

    public void RequestDeviceInfo(string deviceId, Action<IRestResponse<DeviceData>> callback)
    {
        var request = new RestRequest
        {
            Resource = "api/reading/{device_id}"
        };
        request.AddParameter("device_id", deviceId, ParameterType.UrlSegment);
        ExecuteAsync(request, callback);
    }

    public void RequestDeviceList(Action<IRestResponse<List<DeviceId>>> callback)
    {
        var request = new RestRequest
        {
            Resource = "api/device"
        };
        ExecuteAsync(request, callback);
    }

    public void RequestReadingsFrom(string deviceId, long fromUnixTime, Action<IRestResponse<DeviceHistory>> callback)
    {
        var request = new RestRequest
        {
            Resource = "api/reading/{id}/from/{from}",
            //Resource = string.Format("api/reading/{0}/from/{1}", deviceId, fromUnixTime),
            RequestFormat = DataFormat.Json,
            Method = Method.GET
        };
        request.AddParameter("id", deviceId, ParameterType.UrlSegment);
        request.AddParameter("from", fromUnixTime, ParameterType.UrlSegment);
        Debug.LogFormat("Requesting api/reading/{0}/from/{1}", deviceId, fromUnixTime);
        ExecuteAsync(request, callback);
    }

    public void RequestReadingsFromTo(string deviceId, long fromUnixTime, long toUnixTime, Action<IRestResponse<DeviceHistory>> callback)
    {
        var request = new RestRequest
        {
            Resource = "api/reading/{id}/from/{from}/to/{to}"
        };
        request.AddParameter("id", deviceId, ParameterType.UrlSegment);
        request.AddParameter("from", fromUnixTime, ParameterType.UrlSegment);
        request.AddParameter("to", toUnixTime, ParameterType.UrlSegment);
        Debug.LogFormat("Requesting api/reading/{0}/from/{1}/to/{2}", deviceId, fromUnixTime, toUnixTime);
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
