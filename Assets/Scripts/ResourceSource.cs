using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour, IPlanetAngle
{
    public Resource type;
    public float edgeAngle { get; set; } = 0.0f;
}
