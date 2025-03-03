using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{


    public UnityEvent<Tilemap> OnMapGenerated = new UnityEvent<Tilemap>();


    // This function is called by TerrainGenerator when the map is ready
    public void NotifyMapGenerated(Tilemap tilemap)
    {
        OnMapGenerated.Invoke(tilemap);
    }
}
