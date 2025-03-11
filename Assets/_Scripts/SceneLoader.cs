using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneName; // String field to specify the scene name


    [SerializeField]
    private int sceneIndex; // Field to specify the scene index in the build settings

    public void GoToSceneByName()
    {
        // Check if sceneName is valid
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Load the scene by name
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set.");
        }
    }

    public void GoToSceneByIndex()
    {
        // Load the scene by its build index
        SceneManager.LoadScene(sceneIndex);
    }
}
