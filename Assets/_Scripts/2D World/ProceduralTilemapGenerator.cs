using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ProceduralTilemapGenerator : MonoBehaviour
{

    // Source: https://youtu.be/qNZ-0-7WuS8?si=69THi61AXzEkToRW

    public UnityEvent<Tilemap> OnMapGenerated = new UnityEvent<Tilemap>();


    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private int width = 50; // Width of the map
    [SerializeField]
    private int height = 50; // Height of the map
    [SerializeField]
    private float magnification = 10f; // Perlin noise scale


    private int edgeOffset = 3;

    [SerializeField]
    private FixedSizedContainer<CustomTile> PriorityLevel;

    [SerializeField]
    private int _seed; // Default seed, 0 means random

    private Dictionary<int, CustomTile> tileLayers; // Stores tile layers

    private int x_offset;
    private int y_offset;




    public void GenerateWorld(int width, int height, int seed)
    {
        // Generate a random seed if not set
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(1000, 999999); //Set a random seed if not provided
            _seed = seed;
        }

        

        this.width = width;
        this.height = height;

        // Use the seed to generate consistent terrain
        x_offset = _seed * 2;
        y_offset = _seed * 2;

        Debug.Log($"Using Seed: {seed}");

        CreateTileLayers();
        GenerateTerrain();

    }

    public int GetSeed()
    { 
        return _seed;
    }

    public void SetTile(Object2D object2D)
    {
        SetTile(Convert.ToInt32(object2D.prefabID), (int)object2D.posX, (int)object2D.posY);
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
                Debug.Log($"Added Level: {level}");
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

        // Force update of physics colliders
        StartCoroutine(UpdateTilemapCollider());
    }

    // Coroutine to delay collider update
    private IEnumerator UpdateTilemapCollider()
    {
        yield return new WaitForEndOfFrame(); // Wait for physics update
        tilemap.gameObject.GetComponent<TilemapCollider2D>().enabled = false;
        tilemap.gameObject.GetComponent<TilemapCollider2D>().enabled = true;

        // Invoke the event now that collider is updated
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

        Debug.Log($"GetId({x}, {y}) Perlin: {perlinValue}, Tile ID: {id}");
        return id;
    }


    float GetPerlinNoise(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) / magnification, (y - y_offset) / magnification);
        float bias = Mathf.Pow(raw_perlin, 2); // Squaring makes lower values more frequent
        float scaled_perlin = bias * tileLayers.Count; // Scale it to available tiles

        return Mathf.Clamp(scaled_perlin, 0, tileLayers.Count);
    }


    void SetTile(int tileId, int x, int y)
    {
        if (tileLayers.ContainsKey(tileId))
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileLayers[tileId]);


            if (x > width || x < 0 || y > height || y < 0)
            {
                Debug.LogWarning($"Tile ID {tileId} out of bounds! Skipped at ({x}, {y})");

                return;
            }

            if (tileId == tileLayers.Count)
            {
                Debug.Log($"Placed tile {tileId} at ({x}, {y})");
                return;
            }
        }
        else
        {
            Debug.LogWarning($"Tile ID {tileId} out of range! Skipped at ({x}, {y})");
        }

    }
}
