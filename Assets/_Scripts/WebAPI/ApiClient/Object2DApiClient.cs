using System.Collections.Generic;
using System;
using UnityEngine;

public class Object2DApiClient : Singleton<Object2DApiClient>
{

    public async Awaitable<IWebRequestReponse> ReadObject2Ds(string environmentId)
    {
        string route = "/Environment2D/" + environmentId + "/Object2D";

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendGetRequest(route);
        return ParseObject2DListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateObject2D(Object2D object2D)
    {
        string route = "/Environment2D/" + object2D.EnvironmentID + "/Object2D";
        string data = JsonUtility.ToJson(object2D);

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendPostRequest(route, data);
        return ParseObject2DResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateObject2D(Object2D object2D)
    {
        string route = "/Environment2D/" + object2D.EnvironmentID + "/Object2D";
        string data = JsonUtility.ToJson(object2D);

        return await WebApiClient.Instance.SendPutRequest(route, data);
    }

    public async Awaitable<IWebRequestReponse> DeleteObject(string environmentId, string objectID)
    {
        string route = "/Environment2D/" + environmentId + "/Object2D/" + objectID;
        return await WebApiClient.Instance.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseObject2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Object2D object2D = JsonUtility.FromJson<Object2D>(data.Data);
                WebRequestData<Object2D> parsedWebRequestData = new WebRequestData<Object2D>(object2D);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Object2D> environments = JsonHelper.ParseJsonArray<Object2D>(data.Data);
                WebRequestData<List<Object2D>> parsedData = new WebRequestData<List<Object2D>>(environments);
                return parsedData;
            default:
                return webRequestResponse;
        }
    }
}
