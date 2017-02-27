using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestSharp;
using RestSharp.Deserializers;

public class HeatmapApi
{
    static string _url = "http://34.251.68.107:5000";

    public HeatmapApi()
    {
        Debug.Log("Starting HeatmapApi");
        Debug.LogFormat("URL: {0}", _url);
    }

    public T Execute<T>(RestRequest request) where T : new()
    {
        var client = new RestClient {BaseUrl = new Uri(_url) };

        var response = client.Execute(request);

        return new JsonDeserializer().Deserialize<T>(response);
    }

    public Reading GetReading(long deviceId)
    {
        var request = new RestRequest
        {
            Resource = "api/reading/{device_id}",
            RootElement = "Reading"
        };
        request.AddParameter("device_id", deviceId, ParameterType.UrlSegment);
        return Execute<Reading>(request);
    }
}
