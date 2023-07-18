using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private static CameraControl _singleton;
    public static CameraControl Singleton
    {
        get { return _singleton;}
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
    [Range(5, 100)] [SerializeField] private float zoomMinimum = 5;
    [Range(5, 100)][SerializeField] private float zoomMaximum = 100;
    private void OnEnable()
    {
        Singleton = this;
        _camera = GetComponent<Camera>();
    }
    private void Zoom()
    {
        _camera.orthographicSize = 1;
    }
    private void OnValidate()
    {
        if (zoomMinimum > zoomMaximum)
        {
            Debug.LogWarning("Zoom minimum must be larger than zoom maximum");
            zoomMaximum = (zoomMinimum + zoomMaximum) / 2f;
        }
    }
}
