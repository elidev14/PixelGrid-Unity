using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebApiClient : Singleton<WebApiClient>
{
    //Url of webapi
    public string baseUrl;

    private string token;

    public void SetToken(string token)
    {
        this.token = token;
    }


    public async Awaitable<IWebRequestReponse> SendGetRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest(RequestType.GET, route, "");
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPostRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest(RequestType.POST, route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPutRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest(RequestType.PUT, route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendDeleteRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest(RequestType.DELETE, route, "");
        return await SendWebRequest(webRequest);
    }


    private UnityWebRequest CreateWebRequest(RequestType type, string route, string data)
    {
        string url = baseUrl + route;
        Debug.Log("Creating " + type + " request to " + url + " with data: " + data);

        data = RemoveIdFromJson(data); // Backend throws error if it receiving empty strings as a GUID value.
        var webRequest = new UnityWebRequest(url, type.ToString());
        byte[] dataInBytes = new UTF8Encoding().GetBytes(data);
        webRequest.uploadHandler = new UploadHandlerRaw(dataInBytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        AddToken(webRequest);
        return webRequest;
    }

    private async Awaitable<IWebRequestReponse> SendWebRequest(UnityWebRequest webRequest)
    {
        await webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                string responseData = webRequest.downloadHandler.text;
                return new WebRequestData<string>(responseData);
            default:
                return new WebRequestError(webRequest.error);
        }
    }

    private void AddToken(UnityWebRequest webRequest)
    {
        webRequest.SetRequestHeader("Authorization", "Bearer " + token);
    }

    private string RemoveIdFromJson(string json)
    {
        return json.Replace("\"id\":\"\",", "");
    }

}

[Serializable]
public class Token
{
    public string tokenType;
    public string accessToken;
}


public enum RequestType
{
    GET,
    POST,
    PUT,
DELETE,
}