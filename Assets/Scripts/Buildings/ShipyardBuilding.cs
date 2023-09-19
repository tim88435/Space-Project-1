using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipyardBuilding : Building
{
    private float nextShipBuildTime = 0.0f;
    [SerializeField] private float shipBuildCooldown = 30.0f;
    [SerializeField] private Resource shipResource;
    [SerializeField] private float requiredResourceAmount = 10f;
    private void Start()
    {
        DelayNextBuildTime();
    }
    private void Update()
    {
        if (shipResource.Value < requiredResourceAmount)
        {
            DelayNextBuildTime();
        }
        if (nextShipBuildTime > Time.time)
        {
            return;
        }
        FlockAgent newShip = Instantiate(GameManager.prefabList.shipPrefab, transform.position, Quaternion.identity).GetComponent<FlockAgent>();
        newShip.TeamID = TeamID;
        newShip.targetDestination = transform.parent.position;
        shipResource.Value -= requiredResourceAmount;
        DelayNextBuildTime();
    }
    private void DelayNextBuildTime()
    {
        nextShipBuildTime = Time.time + shipBuildCooldown;
    }
}
