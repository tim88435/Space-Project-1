using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamAI : MonoBehaviour, ITeam
{
    public static Dictionary<int, TeamAI> AIList { get; private set; } = new Dictionary<int, TeamAI>();
    public int Team { get; set; }
    public List<Planet> ownedPlanets = new List<Planet>();
    public List<FlockAgent> ownedShips = new List<FlockAgent>();
    private float actionTime = 0;
    [SerializeField] private float actionCooldownSeconds = 7.5f;
    private void Start()
    {
        actionTime = Time.time;
        if (AIList.Keys.Contains(Team))
        {
            if (AIList[Team] != this)
            {
                Debug.LogWarning("Duplicate AI assinged to same team\nDeleting duplicate");
                Destroy(this);
                return;
            }
        }
        AIList.Add(Team, this);
    }
    private void Update()
    {
        if (actionTime > Time.time)
        {

        }
        actionTime = GetNextActionTime();
        //try action
        AIAction[] aIActions = GetAllAIActions();
    }
    private void OnDestroy()
    {
        AIList.Remove(Team);
    }
    private float GetNextActionTime()
    {
        return Time.time + Random.Range(actionCooldownSeconds / 3 * 2, actionCooldownSeconds / 3 * 4);
    }
    private AIAction[] GetAllAIActions()
    {
        //return GetAllFleetMovements();
        //return null;
        return GetAllBuildingActions();

        AIAction[] GetAllBuildingActions()
        {
            if (ownedPlanets.Count == 0)
            {
                return null;
            }
            List<AIAction> AIActions = new List<AIAction>();
            for (int i = 0; i < GameManager.prefabList.buildingZones.Length; i++)
            {
                TryPlaceOnPlanet(ownedPlanets[0], GameManager.prefabList.buildingZones[i], out AIAction x);
                AIActions.Add(x);
            }
            return AIActions.ToArray();
        }
    }
    private bool TryPlaceOnPlanet(Planet planet, BuildingZone buildingZone, out AIAction aIAction)
    {
        aIAction = default;
        if (planet == null) return false;
        if (buildingZone == null) return false;
        Building building = buildingZone.prefab.GetComponent<Building>();
        int tries = (int)(planet.Diameter * planet.Diameter);
        for (int i = 0; i < tries; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, 360 * (i / tries));
            //buildings
            if (planet.AnythingIntersecting(rotation, buildingZone.prefab.transform.lossyScale.x))
            {
                continue;
            }
            //required resources
            if (!building.ResourceCheck(planet, rotation, buildingZone.prefab.transform.lossyScale.x))
            {
                continue;
            }

            Vector3 buildingPosition = (rotation * Vector3.up - planet.transform.position).normalized * planet.ZoneDistanceFromPlanetCentre(buildingZone.prefab.transform.lossyScale.x);
            buildingPosition += planet.transform.position;
            aIAction = new BuildAction(buildingZone, planet, buildingPosition);
            return true;
        }
        return false;
    }
}