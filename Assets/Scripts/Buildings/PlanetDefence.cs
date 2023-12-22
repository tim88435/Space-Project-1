using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class PlanetDefence : Building, IWeapon
{
    public float DamagePerHit { get; set; } = 3.0f;
    public float CooldownSeconds { get; set; } = 1.0f;
    public float NextAttackTime { get; set; } = 0.0f;
    public float Range { get; set; } = 10.0f;

    private void Update()
    {
        if (NextAttackTime > Time.time)
        {
            return;
        }
        FlockAgent enemyShipInRange = Physics2D.OverlapCircleAll(transform.position, Range)
            .Where(x => FlockAgent.ships.ContainsKey(x))// && FlockAgent.ships[x].Team != Team &&)
            .Select(x => FlockAgent.ships[x])
            .Where(x => x.TeamID != TeamID)
            .Where(x => Vector3.Distance(x.transform.position, transform.position) < Range)
            .FirstOrDefault();
        if (enemyShipInRange == null)
        {
            return;
        }
        //try to attack
        Vector3 hitPosition = enemyShipInRange.transform.position;
        if (!((IDamagable)enemyShipInRange).TryHitWith(this))
        {
            hitPosition.x += Random.Range(0.66f, 2) * Random.Range(0, 2) * 2 - 1;
            hitPosition.y += Random.Range(0.66f, 2) * Random.Range(0, 2) * 2 - 1;
        }
        DrawLaser(hitPosition);
    }
    private void DrawLaser(Vector3 hitPosition)
    {
        LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.startColor = GameManager.Singleton.teamColours[TeamID];
        lineRenderer.endColor = GameManager.Singleton.teamColours[TeamID];
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = GameManager.Singleton.defaultLineMaterial;
        lineRenderer.SetPositions(new Vector3[] { transform.position, hitPosition });
        Destroy(lineRenderer.gameObject, 0.5f);
    }
}
