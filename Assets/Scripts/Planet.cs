using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour, ITeam
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 100f;
    public float MaxHealth { get; set; } = 100f;

    [SerializeField] private SpriteRenderer outlineRenderer;
    private List<Building> buildings = new List<Building>();
    private void OnEnable()
    {
        if (outlineRenderer == null)
        {
            Debug.LogWarning($"Outline renderer not attached to {gameObject.name}");
        }
    }
    private void Update()
    {
        FlockAgent[] shipsInRange = OrbitingShips();
        float healthChange = PlanetDamage(shipsInRange);
        if (buildings.Count != 0)
        {
            buildings[0].Health = Mathf.Clamp(buildings[0].Health + healthChange * Time.deltaTime * 2.0f, 0.0f, 10.0f);
            healthChange = Mathf.Max(healthChange, 0.0f);//if there are buildings, planet does not take damage
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
        if (Team == 1)//TODO: revamp this with art
        {
            outlineRenderer.color = Color.cyan;
        }
        else
        {
            outlineRenderer.color = Color.magenta;
        }
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
}
