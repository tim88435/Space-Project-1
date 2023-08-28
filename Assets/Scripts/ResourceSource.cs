using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class ResourceSource : MonoBehaviour, IPlanetAngle, IHoverable
{
    public Resource type;
    public float edgeAngle { get; set; } = 0.0f;

    public string Name => type.name + " Source";

    public string Description => type.Description;
    void OnMouseEnter()
    {
        UIManager.Singleton.hoveredOver.Add(this);
    }
    void OnMouseExit()
    {
        UIManager.Singleton.hoveredOver.Remove(this);
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //not a UI element so this won't be called
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //not a UI element so this won't be called
    }
}
