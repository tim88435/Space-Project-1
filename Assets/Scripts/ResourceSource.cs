using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public class ResourceSource : MonoBehaviour, IPlanetAngle, IHoverable
{
    public Resource type;
    public float edgeAngle { get; set; } = 0.0f;

    public string Name => type.name + " Source";

    public string Description => type.Description;
    public void OnMouseEnter()
    {
        HoverObject.hoveredOver.Add(this);
    }
    public void OnMouseExit()
    {
        HoverObject.hoveredOver.Remove(this);
    }
}
