using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserAuth : MonoBehaviour
{


    // For debugging purposes

    // public User user;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField registerEmail;
    [SerializeField] private TMP_InputField registerPassword;


    [Header("User API Client")]
    public UserApiClient userApiClient;

    [Header("Auth Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Scene Loaders")]
    [SerializeField] private SceneLoader LoadEnviromentMenuScreen;
    [SerializeField] private SceneLoader LoadAuthScreen;


    [Header("Login button")]
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginButtonText;
    [SerializeField] private GameObject LoginErrorObject;
    [SerializeField] private TMP_Text LoginErrorText;

    [Header("Register button")]
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_Text registerButtonText;
    [SerializeField] private GameObject registeErrorObject;
    [SerializeField] private TMP_Text registeErrorText;



    private bool IsLoggingIn = false;

    private void Start()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }


    public async void Login()
    {
        if (IsLoggingIn) return;

        IsLoggingIn = true;
        loginButtonText.text = "Logging in...";
        loginButton.interactable = false;
        LoginErrorObject.SetActive(false);

        var user = new User
        {
            Email = loginEmail.text,
            Password = loginPassword.text
        };

        // **Validatie**
        string validationMessage = LoginFeedback(user);
        if (validationMessage != "OK")
        {
            LoginErrorText.text = $"({validationMessage})";
            LoginErrorObject.SetActive(true);
            ResetLoginUI();
            return;
        }

        // **Inloggen**
        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login succes!");
                LoadEnviromentMenuScreen.GoToSceneByName();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                LoginErrorText.text = "(Er is een fout opgetreden. Controleer je gebruikersnaam en e-mail en probeer het opnieuw.)";
                LoginErrorObject.SetActive(true);
                ResetLoginUI();
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    // **Login UI Reset functie**
    private void ResetLoginUI()
    {
        IsLoggingIn = false;
        loginButtonText.text = "INLOGGEN";
        loginButton.interactable = true;
    }

    private string LoginFeedback(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
            return "E-mail mag niet leeg zijn.";

        if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Voer een geldig e-mailadres in.";

        if (string.IsNullOrEmpty(user.Password))
            return "Wachtwoord mag niet leeg zijn.";

        return "OK";
    }


    public async void Register()
    {
        if (IsLoggingIn) return;

        IsLoggingIn = true;

        var user = new User
        {
            Email = registerEmail.text,
            Password = registerPassword.text
        };

        registerButtonText.text = "Registreren...";
        registerButton.interactable = false;
        registeErrorObject.SetActive(false);

        // **Validatie**
        string validationMessage = ValidateRegisterInput(user);
        if (validationMessage != "OK")
        {
            registeErrorText.text = $"({validationMessage})";
            registeErrorObject.SetActive(true);
            ResetRegisterUI();
            return;
        }

        // **Registreren**
        IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Register succes!");

                // Eventueel terug naar inlogscherm om login te verifiëren
                LoadAuthScreen.GoToSceneByName();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);

                if (errorMessage.Contains("400"))
                {
                    registeErrorText.text = "Er is een fout opgetreden. Controleer je gebruikersnaam en e-mail en probeer het opnieuw.";
                }
                else
                {
                    registeErrorText.text = "Er is een onbekende fout opgetreden. Probeer het later opnieuw.";
                }

                registeErrorObject.SetActive(true);
                ResetRegisterUI();
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    private void ResetRegisterUI()
    {
        IsLoggingIn = false;
        registerButton.interactable = true;
        registerButtonText.text = "REGISTREREN";
    }

    private string ValidateRegisterInput(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
            return "E-mail mag niet leeg zijn.";

        if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Voer een geldig e-mailadres in.";

        if (string.IsNullOrEmpty(user.Password))
            return "Wachtwoord mag niet leeg zijn.";

        // **Gebruik de PasswordFeedback functie voor wachtwoord validatie**
        string passwordFeedback = PasswordFeedback(user.Password);
        if (passwordFeedback != "OK")
            return passwordFeedback;

        return "OK";
    }

    // **Wachtwoordfeedbackfunctie**
    private string PasswordFeedback(string password)
    {
        if (password.Length < 10)
            return "Wachtwoord moet minstens 10 tekens lang zijn.";

        if (!Regex.IsMatch(password, @"[a-z]"))
            return "Wachtwoord moet minstens 1 kleine letter bevatten.";

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return "Wachtwoord moet minstens 1 hoofdletter bevatten.";

        if (!Regex.IsMatch(password, @"\d"))
            return "Wachtwoord moet minstens 1 cijfer bevatten.";

        if (!Regex.IsMatch(password, @"[\W_]"))
            return "Wachtwoord moet minstens 1 speciaal teken bevatten.";

        return "OK";
    }



}
