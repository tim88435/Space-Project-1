using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour, IPlanetAngle
{
    public Resource.Type type = Resource.Durasteel;
    public float edgeAngle { get; set; } = 0.0f;
}
