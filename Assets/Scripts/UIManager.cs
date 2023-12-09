using Custom;
using Custom.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
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
    //selection
    public static bool BuildingSelected { get { return Singleton._zoneSelected != null; } }
    BuildingZone    _zoneSelected;//selected stuff
    SpriteRenderer  _rendererSelected;//renderer to change the colour
    Building        _buildingSelected;

    [SerializeField] private GameObject _pauseMenu;
    private bool isTryingToPlace = false;
    public static bool multiplePlace = false;
    public static bool isHoveringOverUI = false;
    Color halfred;
    private void OnEnable()
    {
        Singleton = this;
    }
    private void Start()
    {
        halfred = Color.Lerp(Color.red, Color.white, 0.5f);
    }
    private void Update()
    {
        isHoveringOverUI = EventSystem.current.IsPointerOverGameObject();
        if (_rendererSelected == null)
        {
            return;
        }
        _rendererSelected.color = halfred;
        Vector3 mousePosition = CameraControl.Singleton.MouseToWorldPosition();
        if (!GetClosestPlanet(mousePosition, out Planet planet))
        {
            _rendererSelected.transform.position = mousePosition;
            return;
        }
        IPlanetAngle placable = _buildingSelected;
        placable.SetEdgeAngle(_rendererSelected.transform.lossyScale.x / 0.5f, planet.Diameter);
        bool spaceExists = planet.EmptySpaceAt((mousePosition - planet.transform.position).ToEuler().z, placable.edgeAngle, out float angle);
        Vector3 buildingPositionFromPlanet = Quaternion.Euler(Vector3.forward * angle) * Vector3.up * planet.ZoneDistanceFromPlanetCentre(_buildingSelected.transform.lossyScale.x);
        _rendererSelected.transform.position = planet.transform.position + buildingPositionFromPlanet;
        _rendererSelected.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        if (!spaceExists)
        {
            return;
        }
        if (!_buildingSelected.ResourceCheck(planet, _buildingSelected.transform.rotation, _buildingSelected.transform.localScale.x))
        {
            return;
        }
        _rendererSelected.color = GameManager.Singleton.teamColours[1];
        TryPlace(planet);
    }
    private void TryPlace(Planet planet)
    {
        if (_zoneSelected == null)
        {
            return;
        }
        if (!isTryingToPlace)
        {
            return;
        }
        //isTryingToPlace =false;
        if (isHoveringOverUI)
        {
            SelectBuilding(null);
            return;
        }
        PlaceBuilding(planet);
        return;
    }
    public static void TogglePause()
    {
        Time.timeScale = 1 - Time.timeScale;
    }
    private void OnPause(InputValue inputValue)
    {
        if (_pauseMenu.activeSelf) return;
        if (inputValue.isPressed)
        {
            TogglePause();
        }
    }
    public void SelectBuilding(BuildingZone buildingZone)
    {
        if (_rendererSelected != null)
        {
            Destroy(_rendererSelected.gameObject);
            if (_buildingSelected && HoverObject.hoveredOver.Contains(_buildingSelected))
            {
                HoverObject.hoveredOver.Remove(_buildingSelected);
            }
        }
        _zoneSelected = buildingZone;
        if (buildingZone != null)
        {
            _rendererSelected = Instantiate(buildingZone.prefab).GetComponent<SpriteRenderer>();
            _rendererSelected.name = buildingZone.name;
            _buildingSelected = _rendererSelected.GetComponent<Building>();
            _buildingSelected.TeamID = 1;
        }
        else
        {
            if (_rendererSelected != null)
            {
                _rendererSelected.enabled = false;
            }
        }
    }
    private void OnMove(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            SelectBuilding(null);
        }
    }
    private void OnSelect(InputValue inputValue)
    {
        isTryingToPlace = inputValue.isPressed;
    }
    private bool GetClosestPlanet(Vector3 position, out Planet planet, float radius = 3.0f)
    {
        planet = null;
        Collider2D[] closeColliders = Physics2D.OverlapCircleAll(position, radius);
        float distance = float.MaxValue;
        for (int i = 0; i < closeColliders.Length; i++)
        {
            if (closeColliders[i].TryGetComponent(out Planet planetInRange))
            {
                if (planet == null)
                {
                    planet = planetInRange;
                    continue;
                }
                if (Vector3.Distance(planetInRange.transform.position, planet.transform.position) < distance)
                {
                    planet = planetInRange;
                }
            }
        }
        if (planet == null)
        {
            return false;
        }
        if (planet.TeamID != 1)
        {
            return false;
        }
        return true; ;
    }
    private void PlaceBuilding(Planet planet)
    {
        if (_zoneSelected == null)
        {
            return;
        }
        //building stuff
        planet.AddBuilding(_buildingSelected);
        _buildingSelected.enabled = true;
        _buildingSelected.transform.parent = planet.transform;
        _rendererSelected.color = GameManager.Singleton.teamColours[1];
        Transform previousBuildingTransform = _rendererSelected.transform;
        _rendererSelected = null;
        if (!multiplePlace)
        {
            SelectBuilding(null);
            return;
        }
        SelectBuilding(_zoneSelected);
        _rendererSelected.transform.position = previousBuildingTransform.position;
        _rendererSelected.transform.rotation = previousBuildingTransform.rotation;
    }
    private void OnSpecificSelect(InputValue inputValue)
    {
        multiplePlace = inputValue.isPressed;
    }
    private void OnPauseMenu(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (!CameraControl.enableCamera && !_pauseMenu.activeSelf) return;
        bool setPaused = !_pauseMenu.activeSelf;
        _pauseMenu.SetActive(setPaused);
        GameManager.Singleton.SetTimeScale(setPaused ? 0 : 1);
        GameManager.Singleton.EnableCameraControl(!setPaused);
    }
    public void LoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void ReloadCurrentScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }
    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
