using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldCreationManager : MonoBehaviour
{
    [SerializeField] TMP_InputField worldName;
    [SerializeField] TMP_InputField width;
    [SerializeField] TMP_InputField height;

    [SerializeField] private SceneLoader LoadEnvironment2D;
    [SerializeField] private SceneLoader LoadBackToMenu;


    // Button text and interaction states
    [SerializeField] private TMP_Text createButtonText;
    [SerializeField] private GameObject creationErrorObject;
    [SerializeField] private TMP_Text creationErrorText;
    [SerializeField] private Button createButton;

    private bool IsCreatingEnvironment = false;

    private void Start()
    {
        //ResetCreationUI();
    }

    public void GenerateWorld()
    {
        // Zorg ervoor dat de gebruiker niet al bezig is met het aanmaken van de wereld
        if (IsCreatingEnvironment) return;

        IsCreatingEnvironment = true;
        createButtonText.text = "Creëren...";
        createButton.interactable = false;
        creationErrorObject.SetActive(false);

        // Probeer de waarden van height en width te converteren
        double parsedHeight, parsedWidth;

        if (!double.TryParse(height.text, out parsedHeight) || !double.TryParse(width.text, out parsedWidth))
        {
            creationErrorText.text = "(Voer geldige getallen in voor hoogte en breedte.)";
            creationErrorObject.SetActive(true);
            ResetCreationUI();
            return;
        }

        // controleer of de wereldnaam uniek is
        if (SessionDataManager.Instance.EnvironmentNameExists(worldName.text))
        {
            creationErrorText.text = "(Deze wereldnaam bestaat al. Kies een andere naam.)";
            creationErrorObject.SetActive(true);
            ResetCreationUI();
            return;
        }

        // maak de environment object
        var environment2D = new Environment2D
        {
            name = worldName.text,
            maxHeight = parsedHeight,
            maxLength = parsedWidth
        };

        // validatie
        string validationMessage = ValidateEnvironmentInput(environment2D);
        if (validationMessage != "OK")
        {
            creationErrorText.text = $"({validationMessage})";
            creationErrorObject.SetActive(true);
            ResetCreationUI();
            return;
        }

        // Toevoegen aan globale lijst van omgevingen
        SessionDataManager.Instance.AddEnvironmentToList(environment2D);

        // Instellen van actieve wereld
        SessionDataManager.Instance.SetCurrentEnvironment(environment2D, true);

        // Laad de scène voor de nieuwe wereld
        LoadEnvironment2D.GoToSceneByName();
    }

    public void ReturnBackToMenu()
    {
        LoadBackToMenu.GoToSceneByName();
    }

    // Reset functie voor de UI
    private void ResetCreationUI()
    {
        IsCreatingEnvironment = false;
        createButtonText.text = "CREËREN";
        createButton.interactable = true;
    }

    // alidatiefunctie voor environment
    private string ValidateEnvironmentInput(Environment2D environment)
    {
        if (string.IsNullOrEmpty(environment.name))
            return "Wereldnaam mag niet leeg zijn.";

        if (environment.name.Length < 1)
            return "Wereldnaam moet minimaal 1 teken bevatten.";

        if (environment.maxHeight < 10)
            return "Hoogte moet groter dan 10 zijn.";

        if (environment.maxLength < 20)
            return "Breedte moet groter dan 20 zijn.";

        if (environment.maxHeight > 100)
            return "Hoogte moet kleiner dan 100 zijn.";

        if (environment.maxLength > 200)
            return "Breedte moet kleiner dan 200 zijn.";

        return "OK";
    }
}
