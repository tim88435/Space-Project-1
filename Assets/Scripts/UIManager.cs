using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public static bool BuildingSelected { get { return Singleton.buildingZoneSelected != null; } }
    BuildingZone buildingZoneSelected;
    SpriteRenderer buildingZoneRenderer;
    Building selectedBuilding;
    SpriteRenderer lastCollidedBuildingChildRenderer;
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
        if (lastCollidedBuildingChildRenderer != null)
        {
            lastCollidedBuildingChildRenderer.color = GameManager.Singleton.teamColours[1];
        }
        if (buildingZoneRenderer == null)
        {
            return;
        }
        Vector3 mousePosition = CameraControl.Singleton.MouseToWorldPosition();
        if (ClosestPlanet(mousePosition, out Planet planet))
        {
            IPlanetAngle placable = selectedBuilding;
            placable.SetEdgeAngle(buildingZoneRenderer.transform.lossyScale.x / 0.5f, planet.Diameter);
            Vector3 buildingPositionFromPlanet = (mousePosition - planet.transform.position).normalized * planet.ZoneDistanceFromPlanetCentre(selectedBuilding.transform.lossyScale.x);
            buildingZoneRenderer.transform.position = planet.transform.position + buildingPositionFromPlanet;
            buildingZoneRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, buildingPositionFromPlanet);
            if (AnotherBuildingIsColliding(selectedBuilding,planet, out lastCollidedBuildingChildRenderer))
            {
                lastCollidedBuildingChildRenderer.color = halfred;
                return;
            }
            if (!selectedBuilding.ResourceCheck(planet, selectedBuilding.transform.rotation, selectedBuilding.transform.localScale.x))
            {
                return;
            }
            TryPlace(planet);
            return;
        }
        buildingZoneRenderer.transform.position = mousePosition;
    }
    public static void TogglePause()
    {
        Time.timeScale = 1 - Time.timeScale;
    }
    private void OnPause(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            TogglePause();
        }
    }
    public void SelectBuilding(BuildingZone buildingZone)
    {
        if (buildingZoneRenderer != null)
        {
            Destroy(buildingZoneRenderer.gameObject);
        }
        buildingZoneSelected = buildingZone;
        if (buildingZone != null)
        {
            buildingZoneRenderer = Instantiate(buildingZone.prefab).GetComponent<SpriteRenderer>();
            buildingZoneRenderer.name = buildingZone.name;
            selectedBuilding = buildingZoneRenderer.GetComponent<Building>();
        }
        else
        {
            if (buildingZoneRenderer != null)
            {
                buildingZoneRenderer.enabled = false;
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
    private void OnDeselect(InputValue inputValue)
    {
        //if (!inputValue.isPressed) { return; }
        //PlayerUnitController.DeselectAllFleets();
    }
    private void OnSelect(InputValue inputValue)
    {
        isTryingToPlace = inputValue.isPressed;
    }
    private void TryPlace(Planet planet)
    {
        if (buildingZoneSelected == null)
        {
            return;
        }
        if (!isTryingToPlace)
        {
            return;
        }
        if (isHoveringOverUI)
        {
            SelectBuilding(null);
            return;
        }
        PlaceBuilding(planet);
        return;
    }
    private bool ClosestPlanet(Vector3 position, out Planet planet)
    {
        planet = null;
        Collider2D[] closeColliders = Physics2D.OverlapCircleAll(position, 3.0f);
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
        if (planet.Team != 1)
        {
            return false;
        }
        return true; ;
    }
    private void PlaceBuilding(Planet planet)
    {
        if (buildingZoneSelected == null)
        {
            return;
        }
        //building stuff
        planet.AddBuilding(selectedBuilding);
        selectedBuilding.enabled = true;
        selectedBuilding.Team = 1;
        selectedBuilding.transform.parent = planet.transform;
        buildingZoneRenderer.color = GameManager.Singleton.teamColours[1];
        Transform previousBuildingTransform = buildingZoneRenderer.transform;
        buildingZoneRenderer = null;
        if (!multiplePlace)
        {
            buildingZoneSelected = null;
            return;
        }
        SelectBuilding(buildingZoneSelected);
        buildingZoneRenderer.transform.position = previousBuildingTransform.position;
        buildingZoneRenderer.transform.rotation = previousBuildingTransform.rotation;
    }
    private bool AnotherBuildingIsColliding(Building building, Planet planet, out SpriteRenderer spriteRenderer)
    {
        if (planet.BuildingsIntersecting(building, out Building[] outBuildings))
        {
            if (lastCollidedBuildingChildRenderer == null)
            {
                spriteRenderer = outBuildings[0].GetComponent<SpriteRenderer>();
                return true;
            }
            for (int i = 0; i < outBuildings.Length; i++)
            {
                if (outBuildings[i].transform == lastCollidedBuildingChildRenderer.transform.parent)
                {
                    spriteRenderer = lastCollidedBuildingChildRenderer;
                    return true;
                }
            }
            spriteRenderer = outBuildings[0].GetComponent<SpriteRenderer>();
            return true;
        }
        spriteRenderer = null;
        return false;
    }
    private void OnSpecificSelect(InputValue inputValue)
    {
        multiplePlace = inputValue.isPressed;
    }
}
