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
    private float ZoomStep;

    private float minCamSize, maxCamSize;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    public void InitializeCamera(BoundsInt bounds)
    {

        minBounds = new Vector2(bounds.min.x, bounds.min.y);
        maxBounds = new Vector2(bounds.max.x, bounds.max.y);

        mapMinX = minBounds.x;
        mapMaxX = maxBounds.x;
        mapMinY = minBounds.y;
        mapMaxY = maxBounds.y;

        float worldWidth = mapMaxX;
        float worldHeight = mapMaxY;

        maxCamSize = worldWidth;

        minCamSize = Mathf.Max(2f, maxCamSize * 0.1f);

        Debug.Log($"Camera orthographicSize: {Camera.orthographicSize}");

        Debug.Log($"World Bounds: minX={mapMinX}, maxX={mapMaxX}, minY={mapMinY}, maxY={mapMaxY}");
        Debug.Log($"Zoom Limits: minCamSize={minCamSize}, maxCamSize={maxCamSize}");

        CenterCameraOnMap();
    }


    private void CenterCameraOnMap()
    {

        if (minBounds == Vector2.zero && maxBounds == Vector2.zero)
        {
            Debug.LogWarning("CameraController: WorldBounds are not set yet.");
            return;
        }

        float centerX = (minBounds.x + maxBounds.x) / 2;
        float centerY = (minBounds.y + maxBounds.y) / 2;

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

    public void ScrollZoom(InputAction.CallbackContext ctx)
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
        float camHeight = Camera.orthographicSize;
        float camWidth = camHeight * Camera.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;


        float x = Mathf.Clamp(targetPosition.x, minX, maxX);
        float y = Mathf.Clamp(targetPosition.y, minY, maxY);
        return new Vector3(x, y, targetPosition.z);
    }


    private Vector3 GetMousePosition => Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}
