using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Custom.Interfaces;
using System.Numerics;

public class ResourceBuilding : Building
{
    [SerializeField] Resource type;
    private void Update()
    {
        type.Value += Time.deltaTime;
    }
    public override bool ResourceCheck(Planet planet)
    {
        return planet.resources
            .Where(x => x != null)
            .Where(x => ((IPlanetAngle)this).IsIntersecting(x))
            .Any(x => x.type == type);
    }
}
