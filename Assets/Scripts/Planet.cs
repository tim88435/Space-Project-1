using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[SelectionBase]
public class Planet : MonoBehaviour, ITeam, IHoverable
{
    public int Team { get; set; } = 0;
    private float health = 100f;
    private float maxHealth = 100f;
    public float Diameter { get { return transform.lossyScale.x; } }

    public string Name { get { return ""; } }

    public string Description { get { return ""; } }

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
        SetTeam();
        SetTeamColour();
        transform.localScale = Vector3.one * (Random.Range(1.0f, 10.0f) + Random.Range(1.0f, 10.0f));
        SetResources();
        healthBar = Instantiate(GameManager.prefabList.healthBarPrefab, transform.position, Quaternion.identity).GetComponent<HealthBar>();
        healthBar.transform.localScale = transform.localScale;
        healthBar.transform.parent = transform;
    }
    private void Update()
    {
        healthBar.gameObject.SetActive(health != maxHealth);
        healthBar.Set(health / maxHealth);
        FlockAgent[] shipsInRange = OrbitingShips();
        float healthChange = PlanetDamage(shipsInRange);
        health = Mathf.Clamp(health + healthChange * Time.deltaTime, 0.0f, maxHealth);
        if (health == 0)
        {
            Team = shipsInRange//give to team with highest ships in orbit
                .GroupBy(i => i.Team)
                .OrderByDescending(grp => grp.Count())
                .First().Key;
            SetTeamColour();
            Instantiate(GameManager.prefabList.circlePrefab, transform.position, Quaternion.identity, transform)
                .GetComponent<SpriteRenderer>().color = GameManager.Singleton.TeamToColour(Team);
        }
    }
    public void OnMouseEnter()
    {
        HoverObject.hoveredOver.Add(this);
    }
    public void OnMouseExit()
    {
        HoverObject.hoveredOver.Remove(this);
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
        if (maxHealth <= 0)
        {
            Debug.LogError("Max health is or below 0");
        }
        if (flockAgents.Length == 0)
        {
            return (50.0f / maxHealth);
        }
        float orbitingValue = 0;//negative is enemy forces, positive is friendly
        for (int i = 0; i < flockAgents.Length; i++)
        {
            orbitingValue += flockAgents[i].Team == Team ? 0.5f : -0.25f;
        }
        orbitingValue = Mathf.Clamp(orbitingValue, -10.0f, 1.0f) * (50.0f / maxHealth);
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
    public bool BuildingsIntersecting(Building buildingToCheck, out Building[] buildingsCollidered)
    {
        buildingsCollidered = buildings.Where(x => ((IPlanetAngle)x).IsIntersecting(buildingToCheck)).ToArray();
        return buildingsCollidered.Length > 0;
    }
    public bool AnythingIntersecting(Quaternion rotation, float width)
    {
        float edgeAngle = Mathf.Asin(width / 2 / (Diameter / 2)) * Mathf.Rad2Deg;
        return buildings.Any(x => ((IPlanetAngle)x).IsIntersecting(rotation, width));
    }
    private void SetResources()
    {
        if (GameManager.Singleton.resourceData.Length == 0)
        {
            return;
        }
        resources = new ResourceSource[(int)(Diameter / 1.5f)];
        for (int i = 0; i < resources.Length; i++)
        {
            GameManager.ResourceData resourceData = GameManager.Singleton.resourceData[Random.Range(0, GameManager.Singleton.resourceData.Length)];
            float edgeAngle = Mathf.Asin(resourceData.resourcePrefab.transform.lossyScale.x / 2 / (Diameter / 2)) * Mathf.Rad2Deg;
            for (int t = 0; t < 10; t++)//try 10 times
            {
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                if (resources.Where(x => x != null).Any(x => ((IPlanetAngle)x).IsIntersecting(randomRotation, edgeAngle)))
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
                    placable.SetEdgeAngle(resource.transform.lossyScale.x / 0.5f, Diameter);
                    break;
                }
            }
        }
    }
    public float ZoneDistanceFromPlanetCentre(float width)//TODO: fix distance
    {
        return Mathf.Sqrt((Diameter * Diameter - width * width) / 4.0f) + width * 0.5f - 0.02f;
    }
    private void SetTeam()
    {
        FlockAgent[] shipsInRange = OrbitingShips();
        if (shipsInRange.Length == 0) return;
        int newTeamID = shipsInRange//give to team with highest ships in orbit
                .GroupBy(i => i.Team)
                .OrderByDescending(grp => grp.Count())
                .First().Key;
        if (newTeamID == Team)
        {
            return;
        }
        ITeam team = this;
        team.AI?.ownedPlanets.Remove(this);
        Team = newTeamID;
        team.AI?.ownedPlanets.Insert(0, this);
    }
}
