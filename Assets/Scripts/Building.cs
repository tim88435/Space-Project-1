using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, ITeam
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 10f;
    [SerializeField] private BuildingZone zone;
    private void Start()
    {
        
    }
}