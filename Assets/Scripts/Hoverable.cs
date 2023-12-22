using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverable : MonoBehaviour, IHoverableUI
{
    [SerializeField] private Object hoverable;
    public string Name => ((IHoverable)hoverable)?.Name ?? "No Name";
    public string Description => ((IHoverable)hoverable)?.Description ?? "No Description";
    private void OnValidate()
    {
        if (hoverable is IHoverable)
        {
            return;
        }
        if (hoverable is not GameObject)
        {
            hoverable = null;
            return;
        }
        if ((hoverable as GameObject).TryGetComponent(out IHoverable a))
        {
            hoverable = a as Object;
            return;
        }
        hoverable = null;
    }
}
