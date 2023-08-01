using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public static void SetDestination(Vector3 position, List<FlockAgent> flockAgents)
    {
        float targetRatio = 2.0f;
        Vector2Int endSize = new Vector2Int(Mathf.RoundToInt(Mathf.Sqrt(flockAgents.Count * targetRatio)), Mathf.RoundToInt(Mathf.Sqrt(flockAgents.Count / targetRatio)));
        if (endSize.x * endSize.y < flockAgents.Count)
        {
            endSize.y++;
        }
        Quaternion up = GetFormationUp(position, flockAgents);
        Vector3 offset = -(Vector2)endSize;
        offset *= 0.5f;
        offset.y += 0.5f;
        offset.x += 0.5f;
        int flockIndex = 0;
        for (int j = 0; j < endSize.y; j++)
        {
            if (flockIndex + endSize.x > flockAgents.Count)
            {
                for (int i = 0; i < endSize.x; i++)
                {
                    if (flockIndex >= flockAgents.Count) { break; }
                    flockAgents[flockIndex].targetDestination = position + up * (new Vector3(i + (endSize.x * endSize.y - flockAgents.Count) * 0.5f, j, 0.0f) + offset);
                    flockIndex++;
                }
            }
            for (int i = 0; i < endSize.x; i++)
            {
                if (flockIndex >= flockAgents.Count) { break; }
                flockAgents[flockIndex].targetDestination = position + up * (new Vector3(i, j, 0.0f) + offset);
                flockIndex++;
            }
        }
        for (int i = 0; i < flockAgents.Count; i++)
        {
            flockAgents[i].lookEndDirection = flockAgents[i].targetDestination + up * Vector3.up;
        }
    }
    private static Quaternion GetFormationUp(Vector3 lookPosition, List<FlockAgent> flockAgents)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < flockAgents.Count; i++)
        {
            result += flockAgents[i].transform.position;
        }
        result /= flockAgents.Count;
        lookPosition -= result;
        return Quaternion.LookRotation(Vector3.forward, lookPosition);
    }
}