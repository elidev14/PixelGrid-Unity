using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ProceduralTilemapGenerator : MonoBehaviour
{

    // Source: https://youtu.be/qNZ-0-7WuS8?si=69THi61AXzEkToRW, 



    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int width = 50; // Width of the map
    [SerializeField]
    private int height = 50; // Height of the map
    [SerializeField]
    private float magnification = 10f; // Perlin noise scale

    [SerializeField]
    private List<RuleTile> RuleTiles; // List of tiles representing layers

    [SerializeField]
    private float seed = 0; // Default seed, 0 means random

    private Dictionary<int, RuleTile> tileLayers; // Stores tile layers

    private float x_offset;
    private float y_offset;



    private float storeSeed = 0; // Default seed, 0 means random

    void Start()
    {
        // Generate a random seed if not set
        if (seed == 0)
        {
            seed = Random.Range(1000, 9999999999999999999); // Set a random seed if not provided
        }

        // For debugging purposes
        storeSeed = seed;

        // Use the seed to generate consistent terrain
        x_offset = seed * 2;
        y_offset = seed * 3;

        Debug.Log($"Using Seed: {seed}");
        CreateTileLayers();
        GenerateTerrain();

    }

    // only for debugging purposes
    void Update()
    {
        // Regenerate terrain if the seed changes
        if (storeSeed != seed)
        {
            storeSeed = seed;
            x_offset = seed * 2;
            y_offset = seed * 3;
            GenerateTerrain();
        }


    }

    private void CreateTileLayers()
    {
        tileLayers = new Dictionary<int, RuleTile>();

        for (var i = 0; i < RuleTiles.Count; i++)
        {
            tileLayers.Add(i, RuleTiles[i]);
        }

        Debug.Log($"Initialized {tileLayers.Count} tile layers.");
        // Notify the GameManager that the map is ready

    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileId = GetId(x, y);
                SetTile(tileId, x, y);
            }
        }


        // AdjustTilemapScale();

        gameManager.NotifyMapGenerated(tilemap);
    }

    private void AdjustTilemapScale()
    {
        // Get the Tilemap's current bounds
        BoundsInt bounds = tilemap.cellBounds;

        // Get the size of the tiles in world space
        Vector3 tileSize = tilemap.tileAnchor;

        // Calculate the scale factors based on the number of tiles and tile size
        float widthInWorldUnits = bounds.size.x * tileSize.x;  // Width in world units
        float heightInWorldUnits = bounds.size.y * tileSize.y; // Height in world units

        // Adjust the Tilemap's transform scale
        tilemap.transform.localScale = new Vector3(widthInWorldUnits / bounds.size.x, heightInWorldUnits / bounds.size.y, 1f);

        // Debug log the new scale
        Debug.Log($"Tilemap scale adjusted to: {tilemap.transform.localScale}");
    }

    private int GetId(int x, int y)
    {
        float perlinValue = GetPerlinNoise(x, y);
        int id = Mathf.FloorToInt(perlinValue);
        int edgeOffset = 3;

        // Ensure edges do not get the bottom tile
        if (x < edgeOffset || x > width - edgeOffset || y < edgeOffset || y > height - edgeOffset)
        {
            id = Mathf.Max(1, id);
        }
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            Debug.LogWarning($"Tile placed outside bounds at ({x}, {y})!");
        }

        Debug.Log($"GetId({x}, {y}) Perlin: {perlinValue}, Tile ID: {id}");
        return id;
    }


    float GetPerlinNoise(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) / magnification, (y - y_offset) / magnification);
        float bias = Mathf.Pow(raw_perlin, 2); // Squaring makes lower values more frequent
        float scaled_perlin = bias * tileLayers.Count; // Scale it to available tiles

        return Mathf.Clamp(scaled_perlin, 0, tileLayers.Count - 1);
    }


    void SetTile(int tileId, int x, int y)
    {
        if (tileLayers.ContainsKey(tileId))
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileLayers[tileId]);
            Debug.Log($"Placed tile {tileId} at ({x}, {y})");
        }
        else
        {
            Debug.LogWarning($"Tile ID {tileId} out of range! Skipped at ({x}, {y})");
        }

    }
}
