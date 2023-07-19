using Extensions.Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    /*
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
    */
    private Camera _camera;
    [Range(5, 100)][SerializeField] private float zoomMinimum = 5.0f;
    [Range(5, 100)][SerializeField] private float zoomMaximum = 100.0f;
    [Range(1, 2)][SerializeField] private float zoomSpeed = 1.2f;
    [Range(0, 10)][SerializeField] private float zoomUpdateSpeed = 5.0f;
    [Range(0, 10)][SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Vector2 xCameraBounds = Vector2.up;
    [SerializeField] private Vector2 yCameraBounds = Vector2.up;
    private Vector2 directionInput = Vector2.zero;
    private float scrollDelta = 0;
    private float targetZoom;
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
        //Singleton = this;
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
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetZoom, Time.deltaTime * zoomUpdateSpeed);
    }
    private void Move()
    {
        Vector3 position= transform.position;
        position += (Vector3)directionInput * Time.deltaTime * _camera.orthographicSize * moveSpeed;
        position.Clamp2D(xCameraBounds, yCameraBounds);
        transform.position = position;
    }
    private void OnMovement(InputValue inputValue)
    {
        directionInput = inputValue.Get<Vector2>();
    }
    private void OnZoom(InputValue inputValue)
    {
        scrollDelta = -inputValue.Get<Vector2>().y / 120f;
    }
}
