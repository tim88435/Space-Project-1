using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverable : MonoBehaviour, IHoverable
{
    [SerializeField] private Object hoverable;
    private IHoverable _hoverable;
    public string Name => _hoverable?.Name ?? "No Name";
    public string Description => _hoverable?.Description ?? "No Description";
    private void OnValidate()
    {
        if (hoverable == null)
        {
            _hoverable = null;
            return;
        }
        if (hoverable is GameObject)
        {
            if (!((GameObject)hoverable).TryGetComponent(out _hoverable))
            {
                hoverable = null;
            }
            return;
        }
        if (hoverable is ScriptableObject)
        {
            if (hoverable is IHoverable)
            {
                _hoverable = (IHoverable)(ScriptableObject)hoverable;
                return;
            }
        }
        hoverable = null;
        _hoverable = null;
    }
}
