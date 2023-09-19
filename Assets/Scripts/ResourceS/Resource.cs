using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public class Resource
public class Resource
{
    public struct Type
    {
        string name;
        string description;
    }
    public Type type;
    //public Type type;
    //Resource(Resource resource)
    //{
    //    Max = resource.Max;
    //    Value = resource.Value;
    //    _description = resource._description;
    //}
    Resource(Type type) 
    {
        this.type = type;
    }
    [SerializeField] private float _max = 9999.0f;
    public float Max
    {
        get => _max;
        set
        {
            _max = Mathf.Max(value, 0.0f);
            _value = Mathf.Clamp(value, 0.0f, _max);
        }
    }
    [SerializeField] private float _value = 0.0f;
    public float Value
    {
        get => _value;
        set => _value = Mathf.Clamp(value, 0.0f, _max);
    }
    private void OnValidate()
    {
        Max = Max;
        TeamAI.All[1].Resources.Durasteel.Value = Value;
        ResourceSource resourceSource = new ResourceSource();
        if (resourceSource.type == TeamAI.All[1].Resources.Durasteel.type)
        {

        }
    }
}