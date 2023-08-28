using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[SelectionBase]
public class Planet : MonoBehaviour, ITeam
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 100f;
    public float MaxHealth { get; set; } = 100f;
    public float Radius { get { return transform.lossyScale.x; } }

    [SerializeField] private SpriteRenderer outlineRenderer;
    [HideInInspector] public List<Building> buildings = new List<Building>();
    [HideInInspector] public ResourceSource[] resources = new ResourceSource[0];
    private HealthBar healthBar;

    private void OnEnable()
    {
        if (outlineRenderer == null)
        {
            Debug.LogWarning($"Outline renderer not attached to {gameObject.name}");
        }
    }
    private void Start()
    {
        FlockAgent[] shipsInRange = OrbitingShips();
        if (shipsInRange.Length == 0) return;
        Team = shipsInRange//give to team with highest ships in orbit
                .GroupBy(i => i.Team)
                .OrderByDescending(grp => grp.Count())
                .First().Key;
        SetTeamColour();
        transform.localScale = Vector3.one * (Random.Range(1.0f, 10.0f) + Random.Range(1.0f, 10.0f));
        SetResources();
        healthBar = Instantiate(GameManager.Singleton.healthBarPrefab, transform.position, Quaternion.identity).GetComponent<HealthBar>();
        healthBar.transform.localScale = transform.localScale;
        healthBar.transform.parent = transform;
    }
    private void Update()
    {
        healthBar.gameObject.SetActive(Health != MaxHealth);
        healthBar.Set(Health / MaxHealth);
        FlockAgent[] shipsInRange = OrbitingShips();
        float healthChange = PlanetDamage(shipsInRange);
        if (buildings.Count != 0)
        {
            buildings[0].Health = Mathf.Clamp(buildings[0].Health + healthChange * Time.deltaTime * 2.0f, 0.0f, 10.0f);
            healthChange = Mathf.Max(healthChange, 0.0f);//if there are buildings, planet does not take damage
            return;
        }
        Health = Mathf.Clamp(Health + healthChange * Time.deltaTime, 0.0f, MaxHealth);
        if (Health == 0)
        {
            Team = shipsInRange//give to team with highest ships in orbit
                .GroupBy(i => i.Team)
                .OrderByDescending(grp => grp.Count())
                .First().Key;
            SetTeamColour();
        }
    }

    private void SetTeamColour()
    {
        outlineRenderer.color = GameManager.Singleton.teamColours[Team];
    }
    private FlockAgent[] OrbitingShips()
    {
        return Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.x)//assume it's a circle, right?
            .Where(x => FlockAgent.ships.ContainsKey(x))
            .Select(y => FlockAgent.ships[y])
            .ToArray();
    }
    private float PlanetDamage(FlockAgent[] flockAgents)
    {
        if (MaxHealth <= 0)
        {
            Debug.LogError("Max health is or below 0");
        }
        if (flockAgents.Length == 0)
        {
            return (50.0f / MaxHealth);
        }
        float orbitingValue = 0;//negative is enemy forces, positive is friendly
        for (int i = 0; i < flockAgents.Length; i++)
        {
            orbitingValue += flockAgents[i].Team == Team ? 0.5f : -0.25f;
        }
        orbitingValue = Mathf.Clamp(orbitingValue, -10.0f, 1.0f) * (50.0f / MaxHealth);
        return orbitingValue;
    }
    public void AddBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            return;
        }
        buildings.Add(building);
    }
    public bool BuildingsIntersecting(Building building, out Building[] buildings)
    {
        buildings = this.buildings.Where(x => ((IPlanetAngle)x).IsIntersecting(building)).ToArray();
        return buildings.Length > 0;
    }
    private void SetResources()
    {
        if (GameManager.Singleton.resourceData.Length == 0)
        {
            return;
        }
        resources = new ResourceSource[(int)(Radius / 1.5f)];
        for (int i = 0; i < resources.Length; i++)
        {
            GameManager.ResourceData resourceData = GameManager.Singleton.resourceData[Random.Range(0, GameManager.Singleton.resourceData.Length)];
            float width = resourceData.resourcePrefab.transform.lossyScale.x;
            for (int t = 0; t < 10; t++)//try 10 times
            {
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                if (resources.Where(x => x != null).Any(x => ((IPlanetAngle)x).IsIntersecting(randomRotation, width)))
                {
                    continue;
                }
                else
                {
                    Vector3 buildingPositionFromPlanet = randomRotation * Vector3.up * (ZoneDistanceFromPlanetCentre(resourceData.resourcePrefab.transform.lossyScale.x));
                    ResourceSource resource = Instantiate(resourceData.resourcePrefab, transform.position + buildingPositionFromPlanet, randomRotation).GetComponent<ResourceSource>();
                    resource.transform.parent = transform;
                    resources[i] = resource;
                    IPlanetAngle placable = resource;
                    placable.SetEdgeAngle(resource.transform.lossyScale.x / 0.5f, Radius);
                    break;
                }
            }
        }
    }
    public float ZoneDistanceFromPlanetCentre(float width)//TODO: fix distance
    {
        return Mathf.Sqrt((Radius * Radius - width * width) / 4.0f) + width * 0.5f - 0.02f;
    }
}
