using Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI
{
    public abstract class AIAction
    {
        public float weight;
        public abstract void Execute();
    }
    public class AttackAction : AIAction
    {
        public AttackAction(List<FlockAgent> ships, Vector3 position)
        {
            this.ships = ships;
            this.position = position;
        }
        List<FlockAgent> ships;
        Vector3 position;
        public override void Execute()
        {
            SetDestination(ships, position);
        }
        private void SetDestination(List<FlockAgent> ships, Vector3 position)
        {
            Flock.SetDestination(position, ships);
        }
    }
    public class BuildAction : AIAction
    {
        public BuildAction(Building building)
        {
            this.building = building;
            weight = TeamAI.OrderOnType(building);//TODO: change to be more dynamic
        }
        internal Building building;
        public override void Execute()
        {
            building.enabled = true;
            building.gameObject.SetActive(true);
        }
    }
}
