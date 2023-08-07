using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehaviour : ScriptableObject
{
    public abstract Vector2 CalculateMove(FlockAgent agent, FlockAgent[] friendlyShips, FlockAgent[] enemyShips);
}
