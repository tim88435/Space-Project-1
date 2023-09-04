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
    void OnMouseEnter()
    {
        HoverObject.hoveredOver.Add(this);
    }
    void OnMouseExit()
    {
        HoverObject.hoveredOver.Remove(this);
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //not a UI element so this won't be called
        Debug.Log("Resource Source getting OnPointerEnter Event!");
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //not a UI element so this won't be called
        Debug.Log("Resource Source getting OnPointerExit Event!");
    }
}
