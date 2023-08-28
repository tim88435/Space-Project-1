using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private void Update()
    {
        foreach (FlockAgent ship in FlockAgent.ships.Values)
        {
            float distance = Vector3.Distance(ship.transform.position, transform.position) - transform.lossyScale.x * 0.5f;
            if (distance > 0)
            {
                return;
            }
            distance *= 0.1f;
            ((IDamagable)ship).DealDirectDamage(distance * Time.deltaTime);
        }
    }
}
