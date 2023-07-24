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
        //Debug.Log(endSize);
        Quaternion up = GetFormationUp(position);
        int flockIndex = 0;
        for (int j = 0; j < endSize.y; j++)
        {
            if (flockIndex + endSize.x > flockAgents.Length)
            {
                for (int i = 0; i < endSize.x; i++)
                {
                    flockAgents[flockIndex].targetDestination = position + up * new Vector3(i / (flockAgents.Length - flockIndex), j, 0.0f);
                    Debug.Log($"{i / (flockAgents.Length - flockIndex)}");
                    flockIndex++;
                }
            }
            for (int i = 0; i < endSize.x; i++)
            {
                flockAgents[flockIndex].targetDestination = position + up * new Vector3(i, j, 0.0f);
                //Debug.Log($"i = {i}\nj = {j}");
                flockIndex++;
            }
        }
    }
    private Quaternion GetFormationUp(Vector3 lookPosition)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < flockAgents.Length; i++)
        {
            result += lookPosition - flockAgents[0].transform.position;
        }
        result /= flockAgents.Length;
        return Quaternion.LookRotation(Vector3.forward, result);
    }
}