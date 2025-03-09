using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Environment2DManager : MonoBehaviour
{
    [SerializeField] private ProceduralTilemapGenerator ProceduralTilemapGenerator;


    private List<TileScriptableObject> tiles;
    

    private void Start()
    {

        //Statemachine
        tiles = new List<TileScriptableObject>();

        if (SessionData.Instance.GetCurrentEnvironmentSessionData().IsNewWorld)
        {
            ProceduralTilemapGenerator.GenerateWorld();
        }else
        {
            GenerateWorld();
        }

    }

    private async void GenerateWorld()
    {


        IWebRequestReponse webRequestResponse = await Object2DApiClient.Instance.ReadObject2Ds(SessionData.Instance.GetCurrentEnvironmentSessionData().Environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                var currentEnvironment2d = SessionData.Instance.GetCurrentEnvironmentSessionData().Environment2D;
                foreach (var object2d in dataResponse.Data)
                {
                    ProceduralTilemapGenerator.GenerateWorld(object2d, currentEnvironment2d);
                }

                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void SaveWorld()
    {
        if (tiles == null) return;
        // Show a verification window for if ur sure to save


        foreach(var TileScriptableObject in tiles)
        {
            if (TileScriptableObject == null) continue;

            var object2d = new Object2D
            {
                ID = TileScriptableObject.ID,
                EnvironmentID = TileScriptableObject.EnvironmentID,
                PosX = TileScriptableObject.PosX,
                PosY = TileScriptableObject.PosY,
                PrefabID = TileScriptableObject.PrefabID,
                RotationZ = TileScriptableObject.RotationZ,
                ScaleX = TileScriptableObject.ScaleX,
                ScaleY = TileScriptableObject.ScaleY,
                SortingLayer = TileScriptableObject.SortingLayer,
            };

            Debug.Log($"Created Object2D: " +
                   $"\nID: {object2d.ID}" +
                   $"\nEnvironmentID: {object2d.EnvironmentID}" +
                   $"\nPosX: {object2d.PosX}, PosY: {object2d.PosY}" +
                   $"\nPrefabID: {object2d.PrefabID}" +
                   $"\nRotationZ: {object2d.RotationZ}" +
                   $"\nScaleX: {object2d.ScaleX}, ScaleY: {object2d.ScaleY}" +
                   $"\nSortingLayer: {object2d.SortingLayer}");


            if (string.IsNullOrEmpty(object2d.ID))
            {
                await Object2DApiClient.Instance.CreateObject2D(object2d);
            }else
            {
                await Object2DApiClient.Instance.UpdateObject2D(object2d);
            }
          
        }

    }

    public void AddTilesToList(TileScriptableObject scriptableObject)
    {
        tiles.Add(scriptableObject);
    }

}
