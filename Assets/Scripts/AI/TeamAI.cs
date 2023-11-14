using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamAI : MonoBehaviour, ITeamController
{
    public int TeamID { get; set; }
    public List<Planet> ownedPlanets = new List<Planet>();
    public List<FlockAgent> ownedShips = new List<FlockAgent>();
    private float actionTime = 0;
    [SerializeField] private float actionCooldownSeconds = 7.5f;
    private Dictionary<ResourceType, float> _resources;
    public Dictionary<ResourceType, float> resources
    {
        get
        {
            if (_resources == null)
            {
                _resources = ResourceType.GetNewResourceList();
            }
            return _resources;
        }
        set { _resources = value; }
    }

    private void Start()
    {
        actionTime = Time.time;
        if (ITeamController.teamControllers.ContainsKey(TeamID))
        {
            if (ITeamController.teamControllers[TeamID] != this)
            {
                Debug.LogWarning($"TeamAI assinged to team {TeamID} already exists\nDeleting duplicate");
                Destroy(this);
                return;
            }
        }
        ITeamController.teamControllers.Add(TeamID, this);
    }
    private void Update()
    {
        if (actionTime > Time.time)
        {
            return;
        }
        actionTime = GetNextActionTime();
        //try action
        AIAction[] aIActions = GetAllAIActions();
    }
    private void OnDestroy()
    {
        ITeamController.teamControllers.Remove(TeamID);
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

            //weights
            float weight = 1;
            if (building is ResourceBuilding resourceBuilding)
            {
                //TODO: weights
                //weight = resourceBuilding.resourceType;
            }


            return true;
        }
        return false;
    }
}
