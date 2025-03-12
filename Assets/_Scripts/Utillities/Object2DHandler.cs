using UnityEngine;


public class Object2DHandler : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public bool isDragging = false;

    private void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;
        if (!isDragging)
        {
            inventoryManager.ShowMenu();
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = -10;
        return positionInWorld;
    }

    public Object2D GetObjectData(string environmentID)
    {
        return new Object2D
        {
            environmentID = environmentID,
            prefabID = gameObject.name.Replace("(Clone)", "").Trim(),
            posX = transform.position.x,
            posY = transform.position.y,
            rotationZ = transform.eulerAngles.z,
            scaleX = transform.localScale.x,
            scaleY = transform.localScale.y,
            sortingLayer = GetComponent<SpriteRenderer>()?.sortingOrder ?? 0
        };
    }
}
