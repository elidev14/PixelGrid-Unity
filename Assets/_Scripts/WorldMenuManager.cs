using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMenuManager : MonoBehaviour
{


    [SerializeField] private Transform slotSection;

    [SerializeField] private Button CreateNewWorldButton;

    [SerializeField] private Button OpenWorldButton;

    public void Start()
    {
        // Check database for worlds
        InitMenu();
    }

    private async void InitMenu()
    {
        if (slotSection == null)
        {
            Debug.LogError("Slots: Received null !");
            return;
        }

        IWebRequestReponse webRequestResponse = await Environment2DApiClient.Instance.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:

                // Might have too override onClick on CreateNewWorldButton


                List<Environment2D> environment2Ds = dataResponse.Data;

                int availableSlot = 0;
                Debug.Log("List of environment2Ds: ");


                // Add scriptableobject where Environment data is stored
                for (var i = 0; i < environment2Ds.Count; i++)
                {
                    var environment2D = environment2Ds[i];

                    if (environment2D != null)
                    {
                        var gO1 = Instantiate(OpenWorldButton);
                        gO1.transform.parent = slotSection;
                        Debug.Log("Slot [{i}] Activated with OpenWorldButton");
                        availableSlot = i + 1;
                    }

                }

                var gO2 = Instantiate(CreateNewWorldButton);
                gO2.transform.parent = slotSection;


                gO2.onClick?.AddListener(OpenWorld);
                Debug.Log("Slot [{i}] Activated with CreateNewWorldButton");

                // TODO: For evey slot Assign open world button / create new world button and set next slot active when there is a open world button
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }

    }

    public void OpenWorld()
    {
        //Add logic for if the world needs to be generated or is already existent
        SceneManager.LoadScene("CreateWorldScrene");
    }

}

