using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITeamController
{
    public int TeamID { get; set; } = 1;
    private Dictionary<ResourceType, float> _resources;
    public Dictionary<ResourceType, float> resources
    {
        get
        {
            if (_resources == null)
            {
                _resources = ITeamController.GetNewResourceList();
            }
            return _resources;
        }
        set { _resources = value; }
    }
    private void Start()
    {
        if (ITeamController.teamControllers.ContainsKey(TeamID))
        {
            if (ITeamController.teamControllers[TeamID] != this)
            {
                Debug.LogWarning($"Player assinged to team {TeamID} already exists\nDeleting duplicate");
                Destroy(this);
                return;
            }
        }
        ITeamController.teamControllers.Add(TeamID, this);
    }
    private void OnDestroy()
    {
        ITeamController.teamControllers.Remove(TeamID);
    }
}
