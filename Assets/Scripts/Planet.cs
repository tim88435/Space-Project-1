using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour, ITeam
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 10f;
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
        if (buildings.Count == 0)
        {
            float healthRate = Mathf.Clamp(OrbitingShips(out FlockAgent[] shipsInRange), -10.0f, 1.0f) * 10.0f + 1.0f;
            Health = Mathf.Clamp(Health + healthRate * Time.deltaTime * 0.1f, 0.0f, 10.0f);
            if (Health == 0)
            {
                Team = shipsInRange//give to team with highest ships in orbit
                    .GroupBy(i => i.Team)
                    .OrderByDescending(grp => grp.Count())
                    .First().Key;
                SetTeamColour();
            }
            return;
        }
        Health = 100;
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
    private float OrbitingShips(out FlockAgent[] shipsInRange)
    {
        shipsInRange = Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.x)//assume it's a circle, right?
            .Where(x => FlockAgent.ships.ContainsKey(x))
            .Select(y => FlockAgent.ships[y])
            .ToArray();
        if (shipsInRange.Length == 0)
        {
            return 0;
        }
        float orbitingValue = 0;//negative is enemy forces, positive is friendly
        for (int i = 0; i < shipsInRange.Length; i++)
        {
            orbitingValue += shipsInRange[i].Team == Team ? 1 : -0.5f;
        }
        orbitingValue /= shipsInRange.Length;
        return orbitingValue;
    }
}
