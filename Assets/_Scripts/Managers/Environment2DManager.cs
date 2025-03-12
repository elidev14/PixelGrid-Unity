using System;
using System.Collections.Generic;
using UnityEngine;

public class Environment2DManager : MonoBehaviour
{
    [SerializeField] private ProceduralTilemapGenerator ProceduralTilemapGenerator;
    [SerializeField] private SceneLoader LoadWorldMenuScreen;
    [SerializeField] private InventoryManager inventoryManager;

    private bool IsSaving;

    private void Start()
    {


        var sessionData = SessionDataManager.Instance.GetCurrentEnvironmentSessionData();

        if (sessionData.IsNewWorld)
        {
            var env2D = sessionData.Environment2D;
            ProceduralTilemapGenerator.GenerateWorld((int)env2D.maxLength, (int)env2D.maxHeight, 0);
            sessionData.Environment2D.Seed = ProceduralTilemapGenerator.GetSeed();
        }
        else
        {
            GenerateWorld();
        }
    }

    private async void GenerateWorld()
    {
        var currentEnvironment2d = SessionDataManager.Instance.GetCurrentEnvironmentSessionData().Environment2D;
        ProceduralTilemapGenerator.GenerateWorld((int)currentEnvironment2d.maxLength, (int)currentEnvironment2d.maxHeight, currentEnvironment2d.Seed);

        IWebRequestReponse webRequestResponse = await Object2DApiClient.Instance.ReadObject2Ds(currentEnvironment2d.id);

        if (webRequestResponse is WebRequestData<List<Object2D>> dataResponse)
        {
            foreach (var object2d in dataResponse.Data)
            {
                InstantiateObject2D(object2d);
            }
        }
        else if (webRequestResponse is WebRequestError errorResponse)
        {
            Debug.LogError("Error loading objects: " + errorResponse.ErrorMessage);
        }
    }

    public void ReturnBackToMyEnivronmentsMenu()
    {
        var sessionData = SessionDataManager.Instance.GetCurrentEnvironmentSessionData();
        sessionData.ClearPlacedObjects();  // Clear objects when returning
        LoadWorldMenuScreen.GoToSceneByName();
    }



    public async void SaveWorld()
    {
        if (IsSaving) return;

        IsSaving = true;
        var sessionData = SessionDataManager.Instance.GetCurrentEnvironmentSessionData();
        var environmentData = sessionData.Environment2D;

        if (SessionDataManager.Instance.EnvironmentExists(environmentData.id))
        {
            Debug.Log("Environment already exists, skipping creation.");
            SaveObject2D(environmentData.id);
            IsSaving = false;
            return;
        }

        Debug.Log(JsonUtility.ToJson(environmentData));

        IWebRequestReponse webRequestResponse = await Environment2DApiClient.Instance.CreateEnvironment(environmentData);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                var newEnvironment = dataResponse.Data;

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

    private async void SaveObject2D(string environmentID)
    {
        var sessionData = SessionDataManager.Instance.GetCurrentEnvironmentSessionData();
        var placedObjects = sessionData.GetPlacedObjects();

        if (placedObjects.Count == 0)
        {
            Debug.Log("No objects to save.");
            return;
        }

        foreach (var objectToSave in placedObjects)
        {
            Object2DHandler handler = objectToSave.GetComponent<Object2DHandler>();
            if (handler == null) continue; // Ensure valid objects are saved

            Object2D object2D = handler.GetObjectData(environmentID);
            Debug.Log($"Saving Object2D with ID: {object2D.id}");

            IWebRequestReponse response = await Object2DApiClient.Instance.CreateObject2D(object2D);

            switch (response)
            {
                case WebRequestData<Object2D> successResponse:
                    Debug.Log($"Object saved at ({object2D.posX}, {object2D.posY}).");
                    break;
                case WebRequestError errorResponse:
                    Debug.LogError("Error saving object: " + errorResponse.ErrorMessage);
                    break;
                default:
                    throw new NotImplementedException("Unexpected response type: " + response.GetType());
            }
        }

        // sessionData.ClearPlacedObjects();
    }



    private void InstantiateObject2D(Object2D objectData)
    {
        // Ensure the prefab name from objectData matches exactly with one in prefabObjects
        GameObject prefab = inventoryManager.prefabObjects.Find(p => p.name == objectData.prefabID);

        if (prefab != null)
        {
            Debug.Log("Object instantiated");

            // Instantiate the object at the saved position and rotation
            GameObject instance = Instantiate(prefab, new Vector3(objectData.posX, objectData.posY, 0), Quaternion.Euler(0, 0, objectData.rotationZ));
            instance.transform.localScale = new Vector3(objectData.scaleX, objectData.scaleY, 1);

            // Add the Object2DHandler to manage the object
            var object2DHandler = instance.AddComponent<Object2DHandler>();
            object2DHandler.inventoryManager = inventoryManager; // Ensure inventory manager is assigned correctly

            // Add to the current environment's placed objects
            SessionDataManager.Instance.GetCurrentEnvironmentSessionData().AddPlacedObject(instance);
        }
        else
        {
            Debug.LogError($"Prefab with ID {objectData.prefabID} not found in inventory.");
        }
    }


}
