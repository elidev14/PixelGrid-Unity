using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneName; 


    [SerializeField]
    private int sceneIndex; 

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
