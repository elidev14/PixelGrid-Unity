using System;
using System.Collections.Generic;
using UnityEngine;

public class Environment2DApiClient : Singleton<Environment2DApiClient>
{

    public async Awaitable<IWebRequestReponse> ReadEnvironment2Ds(Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D";

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendGetRequest(route);
        IWebRequestReponse Environment2DListResponse = ParseEnvironment2DListResponse(webRequestResponse);

        // Check if callback is not null before invoking
        callback?.Invoke(Environment2DListResponse);

        return Environment2DListResponse;
    }

    public async Awaitable<IWebRequestReponse> CreateEnvironment(Environment2D environment, Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D";
        string data = JsonUtility.ToJson(environment);

        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendPostRequest(route, data);
        IWebRequestReponse Environment2DResponse = ParseEnvironment2DResponse(webRequestResponse);

        // Check if callback is not null before invoking
        callback?.Invoke(Environment2DResponse);

        return Environment2DResponse;
    }

    public async Awaitable<IWebRequestReponse> DeleteEnvironment(string environmentId, Action<IWebRequestReponse> callback = null)
    {
        string route = "/Environment2D/" + environmentId;
        IWebRequestReponse webRequestResponse = await WebApiClient.Instance.SendDeleteRequest(route);

        // Check if callback is not null before invoking
        callback?.Invoke(webRequestResponse);

        return webRequestResponse;
    }


    private IWebRequestReponse ParseEnvironment2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Environment2D environment = JsonUtility.FromJson<Environment2D>(data.Data);
                WebRequestData<Environment2D> parsedWebRequestData = new WebRequestData<Environment2D>(environment);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseEnvironment2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Environment2D> environment2Ds = JsonHelper.ParseJsonArray<Environment2D>(data.Data);
                WebRequestData<List<Environment2D>> parsedWebRequestData = new WebRequestData<List<Environment2D>>(environment2Ds);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }
}
