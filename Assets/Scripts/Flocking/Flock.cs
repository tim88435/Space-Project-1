using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock
{
    public FlockAgent[] flockAgents;
    public Flock(FlockAgent[] flockAgents)
    {
        this.flockAgents = flockAgents;
    }

    internal void SetDestination(Vector3 position)
    {
        float targetRatio = 2.0f;
        Vector2Int endSize = new Vector2Int(Mathf.RoundToInt(Mathf.Sqrt(flockAgents.Length * targetRatio)), Mathf.RoundToInt(Mathf.Sqrt(flockAgents.Length / targetRatio)));
        if (endSize.x * endSize.y < flockAgents.Length)
        {
            endSize.y++;
        }
        Quaternion up = GetFormationUp(position);
        Vector3 offset = -(Vector2)endSize;
        offset *= 0.5f;
        offset.y += 0.5f;
        offset.x += 0.5f;
        //Debug.Log(position + offset);
        int flockIndex = 0;
        for (int j = 0; j < endSize.y; j++)
        {
            if (flockIndex + endSize.x > flockAgents.Length)
            {
                for (int i = 0; i < endSize.x; i++)
                {
                    if (flockIndex >= flockAgents.Length) { break; }
                    flockAgents[flockIndex].targetDestination = position + up * (new Vector3(i + (endSize.x * endSize.y - flockAgents.Length) * 0.5f, j, 0.0f) + offset);
                    flockIndex++;
                }
            }
            for (int i = 0; i < endSize.x; i++)
            {
                if (flockIndex >= flockAgents.Length) { break; }
                flockAgents[flockIndex].targetDestination = position + up * (new Vector3(i, j, 0.0f) + offset);
                //Debug.Log($"i = {i}\nj = {j}");
                flockIndex++;
            }
        }
        for (int i = 0; i < flockAgents.Length; i++)
        {
            flockAgents[i].lookEndDirection = flockAgents[i].targetDestination + up * Vector3.up;
        }
    }
    private Quaternion GetFormationUp(Vector3 lookPosition)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < flockAgents.Length; i++)
        {
            result += flockAgents[0].transform.position;
        }
        result /= flockAgents.Length;
        lookPosition -= result;
        return Quaternion.LookRotation(Vector3.forward, lookPosition);
    }
}