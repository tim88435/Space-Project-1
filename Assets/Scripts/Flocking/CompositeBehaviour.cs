using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    [System.Serializable]
    private struct BehaviourGroup
    {
        public FlockBehaviour behaviour;
        public float weights;
    }
    [SerializeField] private BehaviourGroup[] behaviours;//list of behaviours and weights

    public override Vector2 CalculateMove(FlockAgent agent, FlockAgent[] friendlyShips, FlockAgent[] enemyShips)
    {
        Vector2 move = Vector2.zero;
        foreach (BehaviourGroup weightedBehaviour in behaviours)//adds the direction from each behaviour
        {
            Vector2 partialMove = weightedBehaviour.behaviour.CalculateMove(agent, friendlyShips, enemyShips) * weightedBehaviour.weights;

            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > weightedBehaviour.weights * weightedBehaviour.weights)//adds the weights of each behaviour
                {
                    partialMove.Normalize();
                    partialMove *= weightedBehaviour.weights;
                }
            }
            move += partialMove;
        }
        return move;
    }
}
