using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public abstract class Building : MonoBehaviour, IHoverable, IPlanetAngle, ITeam
{
    public float edgeAngle { get; set; } = 0.0f;

    public string Name { get; }

    public string Description { get; }
    public int TeamID { get; set; } = 0;
    public void OnMouseEnter()
    {
        HoverObject.hoveredOver.Add(this);
    }
    public void OnMouseExit()
    {
        HoverObject.hoveredOver.Remove(this);
    }
    public virtual bool ResourceCheck(Planet planet, Quaternion rotation, float width)
    {
        return true;
    }
}