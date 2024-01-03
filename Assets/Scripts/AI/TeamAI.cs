using Custom.Interfaces;
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
            BuildingZone[] allBuildingZones = Resources.LoadAll<BuildingZone>("Buildings");
            allBuildingZones = allBuildingZones.OrderByDescending(x => OrderOnType(x)).ToArray();
            //TODO
            while (true)
            {
                foreach (BuildingZone zone in allBuildingZones)
                {
                    float edgeAngle = Mathf.Asin(zone.prefab.transform.lossyScale.x / planet.transform.lossyScale.x / 2.0f) * Mathf.Rad2Deg;
                    if (!planet.EmptySpaceAt(0, edgeAngle, out float outAngle))
                    {
                        continue;
                    }
                    if (!zone.prefab.GetComponent<Building>().ResourceCheck(planet, Quaternion.Euler(Vector3.forward * outAngle)))
                    {
                        continue;
                    }
                    Building placedZone = Instantiate(zone.prefab).GetComponent<Building>();
                    //placedZone.transform.parent = planet.transform;
                    placedZone.gameObject.SetActive(true);//TODO: debug, change to false!!
                    IPlanetAngle planetAngle = placedZone;
                    planetAngle.Place(planet, edgeAngle);
                    planetAngle.SetEdgeAngle(planet.Diameter);
                    planet.AddBuilding(placedZone);
                    placedZone.GetComponent<SpriteRenderer>().color = GameManager.Singleton.TeamToColour(TeamID);

                    //placed a building, now try build another building
                    goto Placed;
                }
                //tried to place every building and failed
                break;
                Placed:;
            }
        }
        public void PlanetLost(Planet planet)
        {
            actions.RemoveAll(x => (x as BuildAction)?.building.transform.parent == planet.transform);
        }
        private static int OrderOnType(BuildingZone zone)
        {
            Building building = zone.prefab.GetComponent<Building>();
            switch (building)
            {
                //this is pretty arbitrary
                //TODO: Rework AI build order weights
                case PlanetDefence: return 1;
                case ShipyardBuilding: return 3;
                case ResourceBuilding: return 5;//this will never happen since shipyards will be built instead
                default:
                    throw new System.NotImplementedException($"Building of type {building.GetType()} not implemented in build order");
            }
        }
    }
}