using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
[CreateAssetMenu(menuName = "Resources/New Resource")]
public class Resource : ScriptableObject
{
    [SerializeField] private float _max = 9999.0f;
    public float Max
    {
        get => _max;
        set => _max = Mathf.Max(value, 0.0f);
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
        Value = Value;
    }
    [TextArea(1, 5)]
    [SerializeField] private string _description = "";
    public string Description
    {
        get => _description;
    }
}
