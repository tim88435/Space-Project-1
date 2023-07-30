using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Custom.Extensions;

public class CameraControl : MonoBehaviour
{
    private static CameraControl _singleton;
    public static CameraControl Singleton
    {
        get { return _singleton; }
        set
        {
            if (_singleton == null)
            {
                _singleton = value;
                return;
            }
            if (_singleton != value)
            {
                Debug.LogWarning($"Component {nameof(Singleton)} already exists in current scene\nRemoving duplicate");
            }
        }
    }
    private Camera _camera;
    [Range(5, 100)][SerializeField] private float zoomMinimum = 5.0f;
    [Range(5, 100)][SerializeField] private float zoomMaximum = 100.0f;
    [Range(1, 2)][SerializeField] private float zoomSpeed = 1.2f;
    [Range(0, 10)][SerializeField] private float zoomUpdateSpeed = 5.0f;
    [Range(0, 10)][SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private Vector2 xCameraBounds = new Vector2(-200.0f, 200.0f);
    [SerializeField] private Vector2 yCameraBounds = new Vector2(-100.0f, 100.0f);
    private Vector2 directionInput = Vector2.zero;
    public Vector3 mousePositionScreen = Vector3.zero;
    private float scrollDelta = 0.0f;
    private bool isPanning = false;
    private float targetZoom = 0.0f;
    private void OnValidate()
    {
        if (zoomMinimum > zoomMaximum)
        {
            Debug.LogWarning("Zoom minimum must be larger than zoom maximum");
            zoomMaximum = (zoomMinimum + zoomMaximum) / 2f;
            zoomMinimum = zoomMaximum;
        }
    }
    private void OnEnable()
    {
        Singleton = this;
        _camera = GetComponent<Camera>();
    }
    private void Start()
    {
        targetZoom = _camera.orthographicSize;
    }
    private void Update()
    {
        targetZoom = Mathf.Clamp(targetZoom, zoomMinimum, zoomMaximum);
    }
    private void LateUpdate()
    {
        Zoom();
        Move();
    }
    private void Zoom()
    {
        targetZoom *= Mathf.Pow(zoomSpeed, scrollDelta);
        Vector3 mousePositionWorld = MousePositionWorld();
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * zoomUpdateSpeed);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetZoom, blend);
        transform.position += mousePositionWorld - MousePositionWorld();
    }
    private void Move()
    {
        Vector3 position= transform.position;
        position += (Vector3)directionInput * Time.deltaTime * _camera.orthographicSize * moveSpeed;
        position.Clamp2D(xCameraBounds, yCameraBounds);
        transform.position = position;
    }
    public Vector3 MousePositionWorld()
    {
        return MousePositionWorld(mousePositionScreen);
    }
    public Vector3 MousePositionWorld(Vector3 screenPosition)
    {
        return _camera.ScreenToWorldPoint(screenPosition);
    }
    public Vector3 MousePositionScreen() { return mousePositionScreen; }
    private void OnMovement(InputValue inputValue)
    {
        directionInput = inputValue.Get<Vector2>();
    }
    private void OnZoom(InputValue inputValue)
    {
        scrollDelta = -inputValue.Get<Vector2>().y / 120f;
    }
    private void OnMousePosition(InputValue inputValue)
    {
        Vector2 newMousePosition = inputValue.Get<Vector2>();
        //if (newMousePosition == Vector2.zero) return; //hack to fix issue with application being focused
        if (!isPanning)
        {
            mousePositionScreen = newMousePosition;
            return;
        }
        Vector3 position = transform.position;
        position += (MousePositionWorld(mousePositionScreen) - MousePositionWorld(newMousePosition));
        position.Clamp2D(xCameraBounds, yCameraBounds);
        transform.position = position;
        mousePositionScreen = newMousePosition;
    }
    private void OnPan(InputValue inputValue)
    {
        isPanning = inputValue.Get<float>() > 0;
    }
}
