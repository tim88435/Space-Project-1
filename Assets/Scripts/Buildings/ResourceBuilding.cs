using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Custom.Interfaces;

public class ResourceBuilding : Building
{
    [SerializeField] Resource type;
    [SerializeField] private float baseAmount = 0.1f;
    private void Update()
    {
        type.Value += Time.deltaTime * baseAmount;
    }
    public override bool ResourceCheck(Planet planet, Quaternion rotation, float width)
    {
        edgeAngle = Mathf.Asin(width / 2 / (planet.Diameter / 2)) * Mathf.Rad2Deg;
        return planet.resources
            .Where(x => x != null)
            .Where(x => ((IPlanetAngle)x).IsIntersecting(rotation, edgeAngle))
            .Any(x => x.type == type);
    }
}
