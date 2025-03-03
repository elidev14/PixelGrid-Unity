using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;

    [SerializeField]
    private Camera Camera;

    private bool _isDragging;

    [SerializeField]
    private float ZoomStep, minCamSize, maxCamSize;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    public void SetTilemapBounds(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("CameraController: Received null Tilemap!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        Debug.Log($"Tilemap Bounds: min={bounds.min}, max={bounds.max}");

        Vector3Int minCell = bounds.min;
        Vector3Int maxCell = bounds.max;

        Vector3 minWorld = tilemap.GetCellCenterWorld(minCell) - new Vector3(0.5f, 0.5f, 0);
        Vector3 maxWorld = tilemap.GetCellCenterWorld(maxCell) + new Vector3(0.5f, 0.5f, 0);


        Debug.Log($"Cell Center World: minWorld={minWorld}, maxWorld={maxWorld}");

        mapMinX = minWorld.x;
        mapMaxX = maxWorld.x;
        mapMinY = minWorld.y;
        mapMaxY = maxWorld.y;

        Debug.Log($"Camera Bounds: minX={mapMinX}, maxX={mapMaxX}, minY={mapMinY}, maxY={mapMaxY}");

        CenterCameraOnMap();
    }


    private void CenterCameraOnMap()
    {
        float centerX = (mapMinX + mapMaxX) / 2;
        float centerY = (mapMinY + mapMaxY) / 2;

        Camera.transform.position = new Vector3(centerX, centerY, Camera.transform.position.z);
    }

    public void LateUpdate()
    {
        if (_isDragging)
        {
            _difference = _origin - GetMousePosition;
            Camera.transform.position = ClampCamera(Camera.transform.position + _difference);
        }
    }

    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _origin = GetMousePosition;
        }
        _isDragging = ctx.started || ctx.performed;
    }

    public void Zoom(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>();

        if (scroll > 0)
        {
            ZoomIn();
        }
        else if (scroll < 0)
        {
            ZoomOut();
        }
    }

    public void ZoomIn()
    {
        float newSize = Camera.orthographicSize - ZoomStep;
        Camera.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        Camera.transform.position = ClampCamera(Camera.transform.position);
    }

    public void ZoomOut()
    {
        float newSize = Camera.orthographicSize + ZoomStep;
        Camera.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        Camera.transform.position = ClampCamera(Camera.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = Camera.orthographicSize * 2;
        float camWidth = camHeight * Camera.aspect;

        float minX = mapMinX + camWidth / 2;
        float maxX = mapMaxX - camWidth / 2;
        float minY = mapMinY + camHeight / 2;
        float maxY = mapMaxY - camHeight / 2;

        float x = Mathf.Clamp(targetPosition.x, minX, maxX);
        float y = Mathf.Clamp(targetPosition.y, minY, maxY);
        return new Vector3(x, y, targetPosition.z);
    }


    private Vector3 GetMousePosition => Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}
