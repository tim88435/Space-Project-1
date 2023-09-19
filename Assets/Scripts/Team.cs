using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
//tis a container
public class Team
{
    public static Dictionary<int, Team> List { get; private set; } = new Dictionary<int, Team>();
    public static int lastTeamID = 0;
    public int TeamID { get ; set; }
    public TeamAI TeamAI { get ; private set ; }
    Team(bool isPlayer)
    {
        List.Add(TeamID, this);
        TeamID = lastTeamID++;//next id
        if (!isPlayer)
        {
            TeamAI = new TeamAI();
        }
    }
}