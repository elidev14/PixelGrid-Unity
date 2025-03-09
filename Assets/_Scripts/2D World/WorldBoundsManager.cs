using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class WorldBoundsManager : MonoBehaviour
{

    public UnityEvent<Bounds> OnBoundsCalculated = new UnityEvent<Bounds>();


    public void UpdateBounds()
    {
        var bounds = GetComponent<TilemapCollider2D>().bounds;
        Debug.Log($"Bounds Updated: {bounds}");
        OnBoundsCalculated?.Invoke(bounds);
    }


}
