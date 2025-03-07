using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldCreationManager : MonoBehaviour
{
    public void GenerateWorld()
    {
        SceneManager.LoadScene("2DEnvironment");
    }
}
