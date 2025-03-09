using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] private ProceduralTilemapGenerator terrainGenerator;


    private List<TileScriptableObject> tiles;
    

    private void Start()
    {

        //Statemachine
        tiles = new List<TileScriptableObject>();

        // Check if it is already an existing world or if it should be generated first
        terrainGenerator.GenerateWorld();

    }

    public void SaveWorld()
    {
        if (tiles == null) return;
        // Show a verification window for if ur sure to save


        foreach(var TileScriptableObject in tiles)
        {
            if (TileScriptableObject == null) break;

            var object2d = new Object2D
            {
                ID = TileScriptableObject.ID,
                EnvironmentID = TileScriptableObject.EnvironmentID,
                PosX = TileScriptableObject.PosX,
                PosY = TileScriptableObject.PosY,
                PrefabID = TileScriptableObject.PrefabID,
                RotationZ = TileScriptableObject.RotationZ,
                ScaleX = TileScriptableObject.ScaleX,
                ScaleY = TileScriptableObject.ScaleY,
                SortingLayer = TileScriptableObject.SortingLayer,
            };

            if (string.IsNullOrEmpty(object2d.ID))
            {
                Object2DApiClient.Instance.CreateObject2D(object2d);
            }else
            {
                Object2DApiClient.Instance.UpdateObject2D(object2d);
            }
          
        }

    }

    public void AddTilesToList(TileScriptableObject scriptableObject)
    {
        tiles.Add(scriptableObject);
    }

}
