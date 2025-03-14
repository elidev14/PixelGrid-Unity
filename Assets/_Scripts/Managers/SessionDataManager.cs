using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionDataManager : Singleton<SessionDataManager>
{
    private Dictionary<string, Environment2D> Environment2Ds = new Dictionary<string, Environment2D>();
    private EnvironmentSessionData environmentSessionData = new EnvironmentSessionData();

    public void SetCurrentEnvironment(Environment2D environment2D, bool isNewWorld)
    {
        if (environment2D == null) return;

        environmentSessionData.SetEnvironment(environment2D, isNewWorld);
    }

    public EnvironmentSessionData GetCurrentEnvironmentSessionData()
    {
        return environmentSessionData;
    }

    public void AddEnvironmentToList(Environment2D environment2D)
    {
        if (environment2D == null || string.IsNullOrEmpty(environment2D.id)) return;

        if (Environment2Ds.ContainsKey(environment2D.id))
        {
            Environment2Ds[environment2D.id] = environment2D;
        }
        else
        {
            Environment2Ds.Add(environment2D.id, environment2D);
        }
    }

    public void RemoveEnvironmentFromList(string environmentId)
    {
        Environment2Ds.Remove(environmentId);
    }

    public static void ResetGlobals()
    {
        var instance = Instance;
        if (instance == null) return;

        instance.Environment2Ds.Clear();
        instance.environmentSessionData = new EnvironmentSessionData();
    }

    internal bool EnvironmentExists(string id)
    {
        return !string.IsNullOrEmpty(id) && Environment2Ds.ContainsKey(id);
    }

    public bool EnvironmentNameExists(string name)
    {
        // Controleer of de naam al bestaat in de Environment2Ds dictionary
        return Environment2Ds.Values.Any(env => env.name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}


public class EnvironmentSessionData
{
    public Environment2D Environment2D { get; private set; }
    public bool IsNewWorld { get; private set; }
    private List<GameObject> PlacedObjects = new List<GameObject>();

    public void SetEnvironment(Environment2D environment2D, bool isNewWorld)
    {
        Environment2D = environment2D;
        IsNewWorld = isNewWorld;
        if(PlacedObjects == null) PlacedObjects = new List<GameObject>();
    }

    public void AddPlacedObject(GameObject obj)
    {
        if (obj != null && !PlacedObjects.Contains(obj))
        {
            PlacedObjects.Add(obj);
        }
    }

    public List<GameObject> GetPlacedObjects()
    {
        // Kopie teruggeven 
        return new List<GameObject>(PlacedObjects); 
    }

    public void ClearPlacedObjects()
    {
        PlacedObjects.Clear();
    }
}


