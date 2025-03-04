using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ProceduralTilemapGenerator terrainGenerator;
    [SerializeField] private WorldBoundsManager worldBoundsManager;
    [SerializeField] private CameraController cameraController;



    private void Start()
    {
        //terrainGenerator.OnMapGenerated.AddListener(worldBoundsManager.InitializeBounds);

        //// Listen for when world bounds are calculated
        //worldBoundsManager.OnBoundsCalculated.AddListener(cameraController.InitializeCamera);
    }

}
