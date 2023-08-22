using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipyardBuilding : Building
{
    private float nextShipBuildTime = 0.0f;
    [SerializeField] private float shipBuildCooldown = 30.0f;
    [SerializeField] private GameObject shipPrefab;

    private void Start()
    {
        nextShipBuildTime = Time.time + shipBuildCooldown;
    }
    private void Update()
    {
        if (nextShipBuildTime > Time.time)
        {
            return;
        }
        FlockAgent newShip = Instantiate(shipPrefab, transform.position, Quaternion.identity).GetComponent<FlockAgent>();
        newShip.Team = Team;
        newShip.targetDestination = transform.parent.position;
        nextShipBuildTime = Time.time + shipBuildCooldown;
    }
}
