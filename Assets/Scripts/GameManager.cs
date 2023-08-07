using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;
    public static GameManager Singleton
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
    [System.NonSerialized] public Canvas canvas;
    public Material defaultLineMaterial;
    public FlockBehaviour flockBehaviour;
    private void OnEnable()
    {
        Singleton = this;
    }
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        Application.targetFrameRate = 60;
    }
    private void Update()
    {

    }
}
