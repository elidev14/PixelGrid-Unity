using UnityEngine;

[CreateAssetMenu(fileName = "TileScriptableObject", menuName = "Scriptable Objects/TileScriptableObject")]
public class TileScriptableObject : ScriptableObject
{
    public string ID;
    public string EnvironmentID;
    public string PrefabID;
    public float PosX;
    public float PosY;
    public float ScaleX;
    public float ScaleY;
    public float RotationZ;
    public int SortingLayer;
    public CustomTile customTile;
}
