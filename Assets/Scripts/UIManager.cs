using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    BuildingZone buildingZoneSelected;
    SpriteRenderer buildingZoneRenderer;
    Collider2D selectedBuildingCollider;
    SpriteRenderer lastCollidedBuildingChildRenderer;
    private bool possiblePlacement;
    Color halfred;
    private void Start()
    {
        halfred = Color.Lerp(Color.red, Color.clear, 0.5f);
    }
    private void Update()
    {
        if (lastCollidedBuildingChildRenderer != null)
        {
            lastCollidedBuildingChildRenderer.color = Color.white;
            lastCollidedBuildingChildRenderer = null;
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
            possiblePlacement = true;
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
        return Mathf.Sqrt(radius * radius - width * width / 4.0f) + width * 0.5f;
    }
    public void SelectBuilding(BuildingZone buildingZone)
    {
        PlayerUnitController.DeselectAllFleets();
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
            buildingZoneRenderer.enabled = false;
        }
    }
    private void OnSelect(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        if (buildingZoneSelected == null)
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
            PlaceBuilding();
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
        return planet != null;
    }
    private void PlaceBuilding()
    {
        if (buildingZoneSelected == null)
        {
            return;
        }
        Building building = buildingZoneRenderer.GetComponent<Building>();
        buildingZoneRenderer.GetComponent<Collider2D>().enabled = true;
        //building stuff
        building.enabled = true;
        buildingZoneRenderer.color = Color.cyan;
        buildingZoneRenderer = null;
        buildingZoneSelected = null;
    }
    private bool AnotherBuildingIsColliding(Collider2D collider2D, out SpriteRenderer spriteRenderer)
    {
        Collider2D[] results = new Collider2D[0];
        Physics2D.OverlapCollider(collider2D, new ContactFilter2D(), results);
        Debug.Log(results.Length);
        for (int i = 0; i < results.Length; i++)
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
