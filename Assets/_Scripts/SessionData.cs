using System.Collections.Generic;

public class SessionData : Singleton<SessionData>
{
    private List<Environment2D> Environment2Ds = new List<Environment2D>();

    //private List<TileScriptableObject> TileScriptableObjects = new List<TileScriptableObject>();

    private EnvironmentSessionData environmentSessionData = new EnvironmentSessionData();


    public void SetCurrentEnvironment(Environment2D environment2D, bool IsNewWorld)
    {

        if (environment2D == null || environmentSessionData.Environment2D == environment2D) return;

        environmentSessionData.Environment2D = environment2D;
        environmentSessionData.IsNewWorld = IsNewWorld;
    }

    public EnvironmentSessionData GetCurrentEnvironmentSessionData()
    {
        return environmentSessionData;
    }

    public void AddEnvironmentToList(Environment2D environment2D)
    {
        if (Environment2Ds == null) Environment2Ds = new List<Environment2D>();

        Environment2Ds.Add(environment2D);
    }

    //public static List<TileScriptableObject> GetCurrentTileScriptableObjects()
    //{

    //    if (TileScriptableObjects == null) return null;

    //    return TileScriptableObjects;
    //}

    //public static void AddCurrentTileScriptableObjects(TileScriptableObject tileScriptableObject)
    //{

    //    if (TileScriptableObjects == null) return;

    //    TileScriptableObjects.Add(tileScriptableObject);
    //}

    /// <summary>
    /// Used when logging out
    /// </summary>
    public static void ResetGlobals()
    {

    }

}

public class EnvironmentSessionData
{
    public Environment2D Environment2D;
    public bool IsNewWorld;
}