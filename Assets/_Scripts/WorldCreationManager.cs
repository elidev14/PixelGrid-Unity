using System;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldCreationManager : MonoBehaviour
{

    [SerializeField] TMP_InputField worldName;
    [SerializeField] TMP_InputField width;
    [SerializeField] TMP_InputField height;


    public async void GenerateWorld()
    {
        
        // Input validation


        var Environment2D = new Environment2D { 
            name = worldName.text,
            maxHeight = Convert.ToDouble(height.text),
            maxLength = Convert.ToDouble(width.text)
        };

        IWebRequestReponse webRequestResponse =  await Environment2DApiClient.Instance.CreateEnvironment(Environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:

                 Environment2D = new Environment2D
                {
                    name = dataResponse.Data.name,
                    maxHeight = dataResponse.Data.maxHeight,
                    maxLength = dataResponse.Data.maxLength,
                    id = dataResponse.Data.id,
                 };

               
                // adds a new world to the global Enviroments list
                SessionData.Instance.AddEnvironmentToList(Environment2D);

                // Sets the world as active
                SessionData.Instance.SetCurrentEnvironment(Environment2D, true);

                SceneManager.LoadScene("Environment2D");
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
}
