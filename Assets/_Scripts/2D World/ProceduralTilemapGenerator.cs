using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ProceduralTilemapGenerator : MonoBehaviour
{

    // Source: https://youtu.be/qNZ-0-7WuS8?si=69THi61AXzEkToRW

    public UnityEvent<Tilemap> OnMapGenerated = new UnityEvent<Tilemap>();

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
    public int edgeOffset = 3;

    [SerializeField]
    private FixedSizeContainer<CustomTile> PriorityLevel;

    [SerializeField]
    private float seed = 0; // Default seed, 0 means random

    private Dictionary<int, CustomTile> tileLayers; // Stores tile layers

    private float x_offset;
    private float y_offset;



    void Start()
    {
        // Generate a random seed if not set
        if (seed == 0)
        {
            seed = Random.Range(1000, 9999999999999999999); //Set a random seed if not provided
        }

        // For debugging purposes
        storeSeed = seed;

        // Use the seed to generate consistent terrain
        x_offset = seed * 2;
        y_offset = seed * 2;

        Debug.Log($"Using Seed: {seed}");

        CreateTileLayers();
        GenerateTerrain();

    }


    private float storeSeed = 0; // Default seed, 0 means random
    void Update()
    {
        // Regenerate terrain if the seed changes
        if (storeSeed != seed)
        {
            storeSeed = seed;
            x_offset = seed * 2;
            y_offset = seed * 2;
            GenerateTerrain();
        }
    }

    private void CreateTileLayers()
    {
        tileLayers = new Dictionary<int, CustomTile>();

        for (var i = 0; i < PriorityLevel.Length; i++)
        {
            var level = PriorityLevel[i];
            if (level != null)
            {
                tileLayers.Add(i, level);
            }
        }
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
        OnMapGenerated?.Invoke(tilemap);
    }

    private int GetId(int x, int y)
    {

        float perlinValue = GetPerlinNoise(x, y);
        int id = Mathf.FloorToInt(perlinValue);


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
