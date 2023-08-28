using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources")]
public class Resource : ScriptableObject
{
    [SerializeField] private float _max = 9999.0f;
    public float Max { get => _max; set => _max = value; }
    [SerializeField] private float _min = 0.0f;
    public float Min { get => _min; set => _min = value; }
    [SerializeField] private float _value = 0.0f;
    public float Value { get => _value; set => _value = value; }
}
