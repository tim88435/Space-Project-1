using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour, ITeam
{
    public int Team { get; set; } = 0;
    public float Health { get; set; } = 100f;
}
