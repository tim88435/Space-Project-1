using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
[CreateAssetMenu(menuName = "Buildings/New Zone")]
public class BuildingZone : ScriptableObject, IHoverable
{
    [SerializeField] public GameObject prefab;
    [SerializeField] [TextArea(1, 15)] private string _description;
    public string Name => name;
    public string Description => _description;
}