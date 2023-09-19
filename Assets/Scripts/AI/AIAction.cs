using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AIAction
{
    public float weight;
    public abstract void Execute();
}
public class AttackAction : AIAction
{
    public AttackAction(List<FlockAgent> ships, Vector3 position)
    {
        this.ships = ships;
        this.position = position;
    }
    List<FlockAgent> ships;
    Vector3 position;
    public override void Execute()
    {
        SetDestination(ships, position);
    }
    private void SetDestination(List<FlockAgent> ships, Vector3 position)
    {
        Flock.SetDestination(position, ships);
    }
}
public class BuildAction : AIAction
{
    public BuildAction(BuildingZone buildingZone, Planet planet, Vector3 position)
    {
        this.buildingZone = buildingZone;
        this.planet = planet;
        this.position = position;
    }
    BuildingZone buildingZone;
    Planet planet;
    Vector3 position;
    public override void Execute()
    {
        Build(buildingZone, planet, position);
    }
    public void Build(BuildingZone buildingZone, Planet planet, Vector3 position)
    {
        Building newBuilding = GameObject.Instantiate(buildingZone.prefab, planet.transform).GetComponent<Building>();
        newBuilding.transform.position = position;
        Vector3 buildingPositionFromPlanet = (position - planet.transform.position).normalized * planet.ZoneDistanceFromPlanetCentre(newBuilding.transform.lossyScale.x);
        newBuilding.transform.position = planet.transform.position + buildingPositionFromPlanet;
        newBuilding.transform.rotation = Quaternion.LookRotation(Vector3.forward, buildingPositionFromPlanet);
        newBuilding.enabled = true;
    }
}