using Custom;
using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public class TeamAI : MonoBehaviour, ITeamController
    {
        [SerializeField] private int _teamID = 2;
        public int TeamID { get => _teamID; set => _teamID = value; }
        public List<FlockAgent> ownedShips = new List<FlockAgent>();
        private float actionTime = 0;
        [SerializeField] private float actionCooldownSeconds = 7.5f;
        private List<AIAction> actions = new List<AIAction>();
        private Dictionary<ResourceType, float> _resources;
        public Dictionary<ResourceType, float> resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = ITeamController.GetNewResourceList();
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
        }
        private void OnDestroy()
        {
            ITeamController.teamControllers.Remove(TeamID);
        }
        private float GetNextActionTime()
        {
            return Time.time + Random.Range(actionCooldownSeconds / 3 * 2, actionCooldownSeconds / 3 * 4);
        }
        public void PlanetGained(Planet planet)
        {
            BuildingZone[] allBuildingZones = Extensions.FindAssetsByType<BuildingZone>().ToArray();
            Building currentBuilding = GameObject.Instantiate(allBuildingZones[1].prefab, planet.transform).GetComponent<Building>();
            ((IPlanetAngle)currentBuilding).SetEdgeAngle(currentBuilding.transform.lossyScale.x / 2, planet.transform.lossyScale.x / 2);
            while (planet.EmptySpaceAt(0, currentBuilding.edgeAngle, out float position))
            {

                break;//TODO
            }
        }
        public void PlanetLost(Planet planet)
        {
            actions.RemoveAll(x => (x as BuildAction)?.building.transform.parent == planet.transform);
        }
    }
}