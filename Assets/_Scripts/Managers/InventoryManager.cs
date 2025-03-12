using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject UISideMenu;
    public List<GameObject> prefabObjects;
    private List<GameObject> placedObjects = new List<GameObject>();

    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);
        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);
        Object2DHandler object2D = instanceOfPrefab.GetComponent<Object2DHandler>() ?? instanceOfPrefab.AddComponent<Object2DHandler>();
        object2D.inventoryManager = this;
        object2D.isDragging = true;
        placedObjects.Add(instanceOfPrefab);
    }

    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

}
