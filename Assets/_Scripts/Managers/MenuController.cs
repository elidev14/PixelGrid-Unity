using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private SceneLoader LoadMyWorldsMenu;
    [SerializeField] private SceneLoader LoadSharedWorld;
    public void ShowMyWorlds()
    {
        LoadMyWorldsMenu.GoToSceneByName();
    }

    public void ShowSharedWorlds()
    {
        LoadSharedWorld.GoToSceneByName();
    }
}
