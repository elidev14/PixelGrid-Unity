using NUnit;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Environment2DManager : MonoBehaviour
{
    [SerializeField] private ProceduralTilemapGenerator ProceduralTilemapGenerator;

    [SerializeField] private SceneLoader LoadWorldMenuScreen;

    private bool IsSaving;


    private void Start()
    {

        if (SessionDataManager.Instance.GetCurrentEnvironmentSessionData().IsNewWorld)
        {
            var env2D = SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D;

            ProceduralTilemapGenerator.GenerateWorld((int)env2D.maxLength, (int)env2D.maxHeight, 0);

            SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D.Seed = ProceduralTilemapGenerator.GetSeed();
        }
        else
        {
            GenerateWorld();
        }

        // TODO: When object gets added to the environment save it to a list here so that when the save button get clicked the save button get the object from that list
    }

    private async void GenerateWorld()
    {

        var currentEnvironment2d = SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D;
        ProceduralTilemapGenerator.GenerateWorld((int)currentEnvironment2d.maxLength, (int)currentEnvironment2d.maxHeight, currentEnvironment2d.Seed);

        IWebRequestReponse webRequestResponse = await Object2DApiClient.Instance.ReadObject2Ds(SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:


                foreach (var object2d in dataResponse.Data)
                {
                    ProceduralTilemapGenerator.SetTile(object2d);
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
        if (IsSaving) return;

        IsSaving = true;
        var environmentData = SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D;

        // Check if the environment already exists
        if (SessionDataManager.Instance.EnvironmentExists(environmentData.id))
        {
            Debug.Log("Environment already exists, skipping creation.");
            SaveObject2D(environmentData.id);
            IsSaving = false;
            return;
        }

        Debug.Log(JsonUtility.ToJson(environmentData)); // Log the JSON payload


        IWebRequestReponse webRequestResponse = await Environment2DApiClient.Instance.CreateEnvironment(environmentData);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                var newEnvironment = dataResponse.Data;

                // Add environment to session
                SessionDataManager.Instance.AddEnvironmentToList(newEnvironment);
                SessionDataManager.Instance.SetCurrentEnvironment(newEnvironment, false);

                SaveObject2D(newEnvironment.id);
                break;

            case WebRequestError errorResponse:
                Debug.Log("Error creating environment: " + errorResponse.ErrorMessage);
                break;

            default:
                throw new NotImplementedException("Unhandled response type: " + webRequestResponse.GetType());
        }

        IsSaving = false;
    }


    public void ReturnBackToMenu()
    {
        LoadWorldMenuScreen.GoToSceneByName();
    }

    private async void SaveObject2D(string environmentID)
    {
        var tiles = SessionDataManager.Instance.GetCurrentTileScriptableObjects();
        List<Object2D> objectsToSave = new List<Object2D>();

        // Get existing objects from the API (you can skip checking for identical ones, as the world is static)
        IWebRequestReponse webRequestResponse = await Object2DApiClient.Instance.ReadObject2Ds(environmentID);
        List<Object2D> existingObjects = new List<Object2D>();

        if (webRequestResponse is WebRequestData<List<Object2D>> dataResponse)
        {
            existingObjects = dataResponse.Data;
        }

        foreach (var tile in tiles)
        {
            if (tile == null) // Assuming 'IsPlayerPlaced' flags the objects that the player has placed
                continue;

            var newObject = new Object2D
            {
                environmentID = environmentID,
                posX = tile.PosX,
                posY = tile.PosY,
                prefabID = tile.PrefabID,
                rotationZ = tile.RotationZ,
                scaleX = tile.ScaleX,
                scaleY = tile.ScaleY,
                sortingLayer = tile.SortingLayer,
            };

            // Save only new or modified player-placed objects (if needed, you can add a more advanced check here)
            objectsToSave.Add(newObject);
        }

        if (objectsToSave.Count > 0)
        {
            // Save each object individually
            foreach (var objectToSave in objectsToSave)
            {
                IWebRequestReponse response = await Object2DApiClient.Instance.CreateObject2D(objectToSave);

                switch (response)
                {
                    case WebRequestData<Object2D> successResponse:
                        Debug.Log($"Object at ({objectToSave.posX}, {objectToSave.posY}) saved successfully.");
                        break;
                    case WebRequestError errorResponse:
                        Debug.LogError("Error saving object: " + errorResponse.ErrorMessage);
                        break;
                    default:
                        throw new NotImplementedException("Unhandled response type: " + response.GetType());
                }
            }
        }
        else
        {
            Debug.Log("No player-placed objects to save.");
        }
    }

}
