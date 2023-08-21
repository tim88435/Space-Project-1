using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
    Collider2D selectedBuildingCollider;
    SpriteRenderer lastCollidedBuildingChildRenderer;
    private bool possiblePlacement = false;
    private bool isTryingToPlace = false;
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
        if (lastCollidedBuildingChildRenderer != null)
        {
            lastCollidedBuildingChildRenderer.color = Color.white;
        }
        if (buildingZoneRenderer == null)
        {
            return;
        }
        Vector3 mousePosition = CameraControl.Singleton.MousePositionWorld();
        if (ClosestPlanet(mousePosition, out Planet planet))
        {
            Vector3 buildingPositionFromPlanet = (mousePosition - planet.transform.position).normalized * ZoneDistanceFromPlanetCentre(planet.transform.lossyScale.x * 0.5f, buildingZoneRenderer.transform.lossyScale.x);
            buildingZoneRenderer.transform.position = planet.transform.position + buildingPositionFromPlanet;
            buildingZoneRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, buildingPositionFromPlanet);
            if (AnotherBuildingIsColliding(selectedBuildingCollider, out SpriteRenderer spriteRenderer))
            {
                lastCollidedBuildingChildRenderer = spriteRenderer;
                spriteRenderer.color = halfred;
                possiblePlacement = false;
                return;
            }
            lastCollidedBuildingChildRenderer = null;
            possiblePlacement = true;
            TryPlace(planet);
            return;
        }
        possiblePlacement = false;
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
    private float ZoneDistanceFromPlanetCentre(float radius, float width)
    {
        return Mathf.Sqrt(radius * radius - width * width / 4.0f) + width * 0.5f - 0.01f;
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
            selectedBuildingCollider = buildingZoneRenderer.GetComponent<Collider2D>();
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
        if (!inputValue.isPressed) { return; }
        PlayerUnitController.DeselectAllFleets();
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
        if (!possiblePlacement)
        {
            return;
        }
        if (GameManager.Singleton.isHoveringOverUI)
        {
            SelectBuilding(null);
            return;
        }
        if (possiblePlacement)
        {
            PlaceBuilding(planet);
            return;
        }
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
        Building building = buildingZoneRenderer.GetComponent<Building>();
        buildingZoneRenderer.GetComponent<Collider2D>().enabled = true;
        //building stuff
        planet.AddBuilding(building);
        building.enabled = true;
        building.Team = 1;
        building.transform.parent = planet.transform;
        buildingZoneRenderer.color = Color.cyan;
        buildingZoneRenderer = null;
        buildingZoneSelected = null;
    }
    private bool AnotherBuildingIsColliding(Collider2D collider2D, out SpriteRenderer spriteRenderer)
    {
        List<Collider2D> results = new List<Collider2D>();
        collider2D.enabled = true; //OverlapCollider does not work with a turned off collider
        Physics2D.OverlapCollider(collider2D, new ContactFilter2D().NoFilter(), results);
        collider2D.enabled = false;
        if (lastCollidedBuildingChildRenderer != null)
        {
            if (results.Any(x => lastCollidedBuildingChildRenderer.transform.parent == x.transform))
            {
                spriteRenderer = lastCollidedBuildingChildRenderer;
                return true;
            }
        }
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].TryGetComponent(out Building building))
            {
                spriteRenderer = building.transform.GetChild(0).GetComponent<SpriteRenderer>();
                return true;
            }
        }
        spriteRenderer = null;
        return false;
    }
}
