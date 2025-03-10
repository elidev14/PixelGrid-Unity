using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;

public class Object2DApiClient : Singleton<Object2DApiClient>
{

    internal async Awaitable<IWebRequestReponse> ReadObject2Ds(string environmentId, Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D/" + environmentId + "/Object2D";

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendGetRequest(route);
        callback?.Invoke(webRequestResponse);
        return ParseObject2DListResponse(webRequestResponse);
    }

    internal async Awaitable<IWebRequestReponse> CreateObject2D(Object2D object2D, Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D/" + object2D.environmentID + "/Object2D";
        string data = JsonUtility.ToJson(object2D);

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendPostRequest(route, data);

        callback?.Invoke(webRequestResponse);
        return ParseObject2DResponse(webRequestResponse);
    }

    internal async Awaitable<IWebRequestReponse> UpdateObject2D(Object2D object2D, Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D/" + object2D.environmentID + "/Object2D";
        string data = JsonUtility.ToJson(object2D);

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendPutRequest(route, data);

        callback?.Invoke(webRequestResponse);

        return webRequestResponse;
    }

    internal async Awaitable<IWebRequestReponse> DeleteObject(string environmentID, string objectID, Action<IWebRequestReponse> callback = null)
    {
        if (string.IsNullOrEmpty(environmentID) || string.IsNullOrEmpty(objectID))
            return new WebRequestError("Invalid input: Environment ID or object ID is null");
        string route = "/Environment2D/" + environmentID + "/Object2D/" + objectID;
        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendDeleteRequest(route);

        callback?.Invoke(webRequestResponse);
        return webRequestResponse;
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
