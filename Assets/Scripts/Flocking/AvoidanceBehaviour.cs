using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FlockBehaviour
{
    [SerializeField] private float friendlyRadius = 10f;
    [SerializeField] private float enemyRadius = 10f;
    [SerializeField] private float friendlyStrength = 1f;
    [SerializeField] private float enemyStrength = 1f;
    public override Vector2 CalculateMove(FlockAgent ship, FlockAgent[] friendlyShips, FlockAgent[] enemyShips)
    {
        if (friendlyShips.Length + enemyShips.Length == 0)
        {
            return Vector2.zero;
        }

        Vector2 avoidanceMove = Vector2.zero;

        foreach (FlockAgent otherShip in friendlyShips)
        {
            if (Vector2.Distance(otherShip.transform.position, ship.transform.position) < friendlyRadius)
            {
                avoidanceMove += (Vector2)(ship.transform.position - otherShip.transform.position) * friendlyStrength;
            }
        }
        foreach (FlockAgent otherShip in enemyShips)
        {
            if (Vector2.Distance(otherShip.transform.position, ship.transform.position) < enemyRadius)
            {
                avoidanceMove += (Vector2)(ship.transform.position - otherShip.transform.position) * enemyStrength;
            }
        }
        avoidanceMove /= friendlyShips.Length + enemyShips.Length;

        return avoidanceMove;
    }
}
