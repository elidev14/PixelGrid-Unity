using System;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldCreationManager : MonoBehaviour
{

    [SerializeField] TMP_InputField worldName;
    [SerializeField] TMP_InputField width;
    [SerializeField] TMP_InputField height;

    [SerializeField] private SceneLoader LoadEnvironment2D;

    public void GenerateWorld()
    {

        // Input validation

        var Environment2D = new Environment2D
        {
            name = worldName.text,
            maxHeight = Convert.ToDouble(height.text),
            maxLength = Convert.ToDouble(width.text)
        };

        // adds a new world to the global Enviroments list
        SessionDataManager.Instance.AddEnvironmentToList(Environment2D);

        // Sets the world as active
        SessionDataManager.Instance.SetCurrentEnvironment(Environment2D, true);

        LoadEnvironment2D.GoToSceneByName();

    }
}
