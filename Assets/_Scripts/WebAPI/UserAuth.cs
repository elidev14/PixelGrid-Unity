using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserAuth : MonoBehaviour
{


    // For debugging purposes

    // public User user;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginUsername;
    [SerializeField] private TMP_InputField loginPassword;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField registerUsername;
    [SerializeField] private TMP_InputField registerPassword;


    [Header("User API Client")]
    public UserApiClient userApiClient;

    [Header("Auth Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

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

        var user = new User 
        { 
          Email = loginUsername.text,
          Password = loginPassword.text
        };

        if (CheckInputValidation(user))
        {
            IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

            switch (webRequestResponse)
            {
                case WebRequestData<string> dataResponse:
                    Debug.Log("Login succes!");

                    //replace buttons with loading icon ddisable the buttons

                    SceneManager.LoadScene("Menu Screen");
                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    Debug.Log("Login error: " + errorMessage);
                    IsLoggingIn = false;
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }else
        {
            // TODO: Add red text to inputfield that is null
            if (string.IsNullOrEmpty(loginUsername.text))
            {
                Debug.Log("Username is null or empty");
            }

            if (string.IsNullOrEmpty(loginPassword.text))
            {
                Debug.Log("Passowrd is null or empty");
            }

            IsLoggingIn = false;
        }


    }

    public async void Register()
    {
        var user = new User
        {
            Email = registerUsername.text,
            Password = registerPassword.text
        };

        if (CheckInputValidation(user))
        {

            IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

            switch (webRequestResponse)
            {
                case WebRequestData<string> dataResponse:
                    Debug.Log("Register succes!");


                    // Maybe return back to login panel to verify login
                    SceneManager.LoadScene("Menu Screen");

                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    Debug.Log("Register error: " + errorMessage);
                    // TODO: Handle error scenario. Show the errormessage to the user.
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        else
        {
            // TODO: Add red text to inputfield that is null
            if (string.IsNullOrEmpty(registerUsername.text))
            {
                Debug.Log("Username is null or empty");
            }

            if (string.IsNullOrEmpty(registerPassword.text))
            {
                Debug.Log("Passowrd is null or empty");
            }
        }

    }

    private bool CheckInputValidation(User user)
    { 
        return !string.IsNullOrEmpty(user.Email) || !string.IsNullOrEmpty(user.Password);
    }


}
