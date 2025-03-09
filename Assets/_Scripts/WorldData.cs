using System.Collections.Generic;

public class WorldData
{
    private static List<Environment2D> Environment2Ds = new List<Environment2D>();

    private static List<TileScriptableObject> TileScriptableObjects = new List<TileScriptableObject>();

    private static Environment2D Environment2D;

    private static string _UserID;


    public static void SetCurrentEnvironment(Environment2D environment2D)
    {

        if (environment2D == null) return;

        Environment2D = environment2D;
    }

    public static Environment2D GetCurrentEnvironment()
    {
        return Environment2D;
    }

    public static void AddEnvironmentToList(Environment2D environment2D)
    {
        if (Environment2D == null) Environment2Ds = new List<Environment2D>();

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
        Environment2Ds =  null;
        Environment2D = null;
    }

}
