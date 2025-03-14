using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WorldMenuManager : MonoBehaviour
{
    [SerializeField] private Transform slotSection;
    [SerializeField] private Button CreateNewWorldButton;
    [SerializeField] private Button OpenWorldButton;
    [SerializeField] private Button EditButton; // De edit button die de delete buttons activeert
    [SerializeField] private SceneLoader LoadCreationWorld;
    [SerializeField] private SceneLoader LoadEnvironment2D;
    [SerializeField] private SceneLoader LoadMenu;

    private bool deleteButtonsVisible = false;
    private List<Button> deleteWorldButtons = new List<Button>(); 

    public void Start()
    {
        // initialisatie van het menu
        InitMenu();

        // Veg een listener toe voor de edit Button
        EditButton.onClick.AddListener(ActivateDeleteButtons);
    }

    private async void InitMenu()
    {
        IWebRequestReponse webRequestResponse = await Environment2DApiClient.Instance.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                environment2Ds.Reverse();

                int availableSlots = 5;
                Debug.Log("List of environment2Ds: ");

                foreach (var environment2D in environment2Ds)
                {
                    if (environment2D != null)
                    {
                        // Voegt de omgeving toe aan de lijst
                        SessionDataManager.Instance.AddEnvironmentToList(environment2D); 
                        var worldButton = Instantiate(OpenWorldButton);
                        worldButton.GetComponentInChildren<TextMeshProUGUI>().text = environment2D.name;

                        // voegt de delete button toe en verberg deze in eerste instantie
                        Button deleteButton = worldButton.transform.Find("DeleteWorldButton").GetComponent<Button>();
                        deleteButton.gameObject.SetActive(false); // Verberg de delete button eerst
                        deleteButton.onClick?.AddListener(() => DeleteWorld(environment2D, worldButton));

                        // voegt de open world listener toe
                        worldButton.onClick?.AddListener(() => OpenWorld(environment2D));
                        worldButton.transform.SetParent(slotSection);

                        // voegt de delete button toe aan de lijst
                        deleteWorldButtons.Add(deleteButton);

                        availableSlots--;
                    }
                }

                if (availableSlots > 0)
                {
                    CreateNewWorldButton = Instantiate(CreateNewWorldButton); // Store reference
                    CreateNewWorldButton.transform.SetParent(slotSection);
                    CreateNewWorldButton.onClick?.AddListener(CreateWorld);
                }

                break;

            case WebRequestError errorResponse:
                Debug.Log("Error loading worlds: " + errorResponse.ErrorMessage);
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }



    private void ActivateDeleteButtons()
    {
        deleteButtonsVisible = !deleteButtonsVisible; // toggle visibility status

        // create a new list that only contains active buttons
        List<Button> activeDeleteButtons = new List<Button>();

        foreach (var deleteButton in deleteWorldButtons)
        {
            if (deleteButton != null) 
            {
                activeDeleteButtons.Add(deleteButton);
                deleteButton.gameObject.SetActive(deleteButtonsVisible); 
            }
        }


        deleteWorldButtons = activeDeleteButtons;

        if (CreateNewWorldButton != null)
        {
            CreateNewWorldButton.gameObject.SetActive(!deleteButtonsVisible); // hide the new world button when delete buttons are visible
        }
    }



    // de wereld verwijderen
    private async void DeleteWorld(Environment2D environment2D, Button worldButton)
    {
        // verwijder de wereld via de API
        await Environment2DApiClient.Instance.DeleteEnvironment(environment2D.id, response =>
        {
            if (response is WebRequestData<string> data && data.Data == "Environment2D object deleted")
            {
                // verwijder de wereld uit de lijst en verwijder de bijbehorende UI
                SessionDataManager.Instance.RemoveEnvironmentFromList(environment2D.id);
                Destroy(worldButton.gameObject);
                deleteWorldButtons.Remove(worldButton.GetComponentInChildren<Button>()); // verwijder de delete button uit de lijst
                Debug.Log("World " + environment2D.name + " deleted successfully.");
            }
        });
    }

    private void CreateWorld()
    {
        LoadCreationWorld.GoToSceneByName();
    }

    private void OpenWorld(Environment2D environment2D)
    {
        SessionDataManager.Instance.SetCurrentEnvironment(environment2D, false);
        LoadEnvironment2D.GoToSceneByName();
    }

    public void LoadMenuScene()
    {
        LoadMenu.GoToSceneByName();
    }


}
