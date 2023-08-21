using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Zone")]
public class BuildingZone : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] public GameObject prefab;
    [SerializeField] [TextArea(1, 15)] private string description;
}