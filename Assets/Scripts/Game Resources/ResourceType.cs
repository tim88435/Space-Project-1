using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Extensions;
[CreateAssetMenu(menuName = "New Resource Type")]
public class ResourceType : ScriptableObject
{
    public Sprite sprite;
    [TextArea]
    public string description;
    public static Dictionary<ResourceType, float> GetNewResourceList()
    {
        Dictionary<ResourceType, float> newList = new Dictionary<ResourceType, float>();
        ResourceType[] resourceTypes = Extensions.FindAssetsByType<ResourceType>().ToArray();
        for (int i = 0; i < resourceTypes.Length; i++)
        {
            newList.Add(resourceTypes[i], 0);
        }
        return newList;
    }
}
