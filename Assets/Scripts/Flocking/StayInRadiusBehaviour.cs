using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius")]
public class StayInRadiusBehaviour : FlockBehaviour
{
    [SerializeField] private float distance = 10f;
    public override Vector2 CalculateMove(FlockAgent ship, FlockAgent[] _, FlockAgent[] enemyShips)
    {
        if (enemyShips.Length == 0)
        {
            return Vector2.zero;
        }

        Vector2 radiusMove = Vector2.zero;

        foreach (FlockAgent otherShip in enemyShips)
        {
            if (Vector2.Distance(otherShip.transform.position, ship.transform.position) > distance)
            {
                radiusMove += (Vector2)(otherShip.transform.position - ship.transform.position);
            }
        }
        radiusMove /= enemyShips.Length;

        return radiusMove;
    }
}
