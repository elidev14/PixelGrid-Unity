using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class WorldBoundsManager : MonoBehaviour
{

    public UnityEvent<BoundsInt> OnBoundsCalculated = new UnityEvent<BoundsInt>();

    public void InitializeBounds(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("WorldBoundsManager: Received null Tilemap!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        OnBoundsCalculated?.Invoke(bounds);
    }

}
