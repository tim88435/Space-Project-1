using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamAI : MonoBehaviour, ITeam
{
    public int Team { get; set; }
    public List<FlockAgent> ownedShips = new List<FlockAgent>();
    private float actionTime = 0;
    [SerializeField] private float actionCooldownSeconds = 7.5f;
    private void Start()
    {
        actionTime = Time.time;
    }
    private void Update()
    {
        if (actionTime > Time.time)
        {

        }
        actionTime = GetNextActionTime();
        //try action
        AIAction[] aIActions = GetAllAIActions();
    }
    private float GetNextActionTime()
    {
        return Time.time + Random.Range(actionCooldownSeconds / 3 * 2, actionCooldownSeconds / 3 * 4);
    }
    private AIAction[] GetAllAIActions()
    {
        //return GetAllFleetMovements();
        return GetAllBuildingActions();

        AIAction[] GetAllBuildingActions()
        {
            for (int i = 0; i < Planet.instances.Count; i++)
            {
                if (Planet.instances[i].Team != Team)
                {
                    continue;
                }

            }
            AIAction[] AIAction = new AIAction[3];
            return AIAction;
        }
    }
}
