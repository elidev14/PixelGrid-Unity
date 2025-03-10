using System;
using System.Collections.Generic;

public class SessionDataManager : Singleton<SessionDataManager>
{
    private Dictionary<string, Environment2D> Environment2Ds = new Dictionary<string, Environment2D>();

    private List<TileScriptableObject> TileScriptableObjects = new List<TileScriptableObject>();

    private EnvironmentSessionData environmentSessionData = new EnvironmentSessionData();

    public void SetCurrentEnvironment(Environment2D environment2D, bool IsNewWorld)
    {
        if (environment2D == null) return;

        environmentSessionData.Environment2D = environment2D;
        environmentSessionData.IsNewWorld = IsNewWorld;
    }

    public EnvironmentSessionData GetCurrentEnvironmentSessionData()
    {
        return environmentSessionData;
    }

    public void AddEnvironmentToList(Environment2D environment2D)
    {
        if (environment2D == null || string.IsNullOrEmpty(environment2D.id)) return;

        if (Environment2Ds == null)
            Environment2Ds = new Dictionary<string, Environment2D>();

        if (Environment2Ds.ContainsKey(environment2D.id))
        {
            Environment2Ds[environment2D.id] = environment2D; // Update existing entry
        }
        else
        {
            Environment2Ds.Add(environment2D.id, environment2D); // Add new entry
        }
    }

    public List<TileScriptableObject> GetCurrentTileScriptableObjects()
    {
        return TileScriptableObjects ?? new List<TileScriptableObject>();
    }

    public void AddToCurrentTileScriptableObjects(TileScriptableObject tileScriptableObject)
    {
        if (tileScriptableObject == null) return;

        TileScriptableObjects.Add(tileScriptableObject);
    }

    /// <summary>
    /// Used when logging out
    /// </summary>
    public static void ResetGlobals()
    {
        var instance = Instance;
        if (instance == null) return;

        instance.Environment2Ds.Clear();
        instance.TileScriptableObjects.Clear();
        instance.environmentSessionData = new EnvironmentSessionData();
    }

    internal bool EnvironmentExists(string id)
    {
        return !string.IsNullOrEmpty(id) && Environment2Ds.ContainsKey(id);
    }
}

public class EnvironmentSessionData
{
    public Environment2D Environment2D;
    public bool IsNewWorld;
}
