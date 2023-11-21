using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;
[CreateAssetMenu(menuName = "New Resource Type")]
public class ResourceType : ScriptableObject
{
    public Sprite sprite;
    [TextArea]
    public string description;
}
