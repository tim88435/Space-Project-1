using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour, ITeam, IPlanetAngle
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 30f;
    public float MaxHealth { get; set; } = 30f;
    public float edgeAngle { get; set; } = 0.0f;
    public virtual bool ResourceCheck(Planet planet)
    {
        return true;
    }
}