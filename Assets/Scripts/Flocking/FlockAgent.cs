using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
using System.Linq;

public class FlockAgent : MonoBehaviour, ISelectable, IDamagable, IWeapon
{
    public static Dictionary<Collider2D, FlockAgent> ships { get; private set; } = new Dictionary<Collider2D, FlockAgent>();
    public bool dogFighting = true;
    public Vector3 lookEndDirection;
    [SerializeField] float speed = 0;
    SpriteRenderer ISelectable.spriteRenderer { get; set; }
    [SerializeField] private int _team = 0;
    public int Team
    {
        get { return _team; }
        set
        {
            _team = value;
            gameObject.layer = 6 + ((ITeam)this).Team;
        }
    }
    public float Health { get; set; } = 10f;
    public float Evasion { get; set; } = 0.3f;
    public float DamagePerHit { get; set; } = 1;
    public float CooldownSeconds { get; set; } = 3;
    public float NextAttackTime { get; set; } = 0;
    public float Range { get; set; } = 15f;

    [System.NonSerialized] public Vector3 targetDestination;
    private void OnEnable()
    {
        Team = Team;//set layer
        ships.Add(GetComponent<Collider2D>(), this);
        ((ISelectable)this).spriteRenderer = GetComponent<SpriteRenderer>();
        if (Team != 1)
        {
            ((ISelectable)this).spriteRenderer.color = Color.magenta;
        }
    }
    private void OnDisable()
    {
        ships.Remove(GetComponent<Collider2D>());//memory leak fix
        if (PlayerUnitController.Singleton.selected.Contains(this))
        {
            PlayerUnitController.Singleton.selected.Remove(this);
        }
    }
    private void Start()
    {
        lookEndDirection = transform.up + transform.position;
        targetDestination = transform.position;
    }
    private void Update()
    {
        if (dogFighting)
        {
            float distance;
            FlockAgent closestFlockAgent;
            if (EnemiesInRange(out Vector3 averagePosition, out Collider2D[] collider2D, Range * 2f))
            {
                distance = Vector3.Distance(targetDestination, averagePosition);
                if (distance < Range * 2.0f)
                {
                    MoveForward(10);//max speed
                    RotateTowards(averagePosition, 0.1f);
                    if (EnemyInRange(out closestFlockAgent, collider2D, Range))
                    {
                        if (NextAttackTime < Time.time)
                        {
                            Attack(closestFlockAgent);
                        }
                    }
                    return;
                }
            }
            distance = Vector2.Distance(transform.position, targetDestination);
            if (distance > 0.1f)
            {
                MoveForward(distance);
                RotateTowards(targetDestination, 1);
            }
            else
            {
                RotateTowards(lookEndDirection, 1);
            }
            if (EnemyInRange(out closestFlockAgent, collider2D, Range))
            {
                if (NextAttackTime < Time.time)
                {
                    Attack(closestFlockAgent);
                }
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, targetDestination);
            if (distance > 0.1f)
            {
                MoveForward(distance);
                RotateTowards(targetDestination, 1);
            }
            else
            {
                RotateTowards(lookEndDirection, 1);
            }
            if (EnemyInRange(out FlockAgent closestFlockAgent, Range))
            {
                if (NextAttackTime < Time.time)
                {
                    Attack(closestFlockAgent);
                }
            }
        }
    }
    private void MoveForward(float distance)
    {
        speed = Mathf.Min(distance * 3.0f, 5.0f);
        transform.Translate(Vector2.up * Time.deltaTime * speed);
    }
    private void RotateTowards(Vector3 location, float strength)
    {
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * speed * 10.0f) * strength;
        Quaternion rotation = Quaternion.LookRotation(transform.forward, location - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, blend);
    }
    private void Attack(FlockAgent agent)
    {
        Vector3 hitPosition = agent.transform.position;
        if (!((IWeapon)this).TryHit(agent))
        {
            hitPosition.x += Random.Range(0.66f, 2) * Random.Range(0, 2) * 2 - 1;
            hitPosition.y += Random.Range(0.66f, 2) * Random.Range(0, 2) * 2 - 1;
        }
        LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = GameManager.Singleton.defaultLineMaterial;
        lineRenderer.SetPositions(new Vector3[] { transform.position, hitPosition });
        Destroy(lineRenderer.gameObject, 0.5f);
    }
    private bool EnemyInRange(out FlockAgent closestFlockAgent, Collider2D[] collider2Ds, float Range)
    {
        if (collider2Ds.Length == 0) { closestFlockAgent = null; return false; }
        Collider2D closestCollider2D = collider2Ds.Aggregate((collider1, collider2) => Vector3.Distance(transform.position, collider2.transform.position) > Vector3.Distance(transform.position, collider2.transform.position) ? collider2 : collider1);
        if (closestCollider2D == null || Vector3.Distance(transform.position, closestCollider2D.transform.position) > Range) { closestFlockAgent = null; return false; }
        closestFlockAgent = ships[closestCollider2D];
        return true;
    }
    private bool EnemyInRange(out FlockAgent closestFlockAgent, float Range)
    {
        Collider2D[] selectedColliders2D = Physics2D.OverlapCircleAll(transform.position, Range, (LayerMask)~(1 << 6 + ((ITeam)this).Team));
        if (selectedColliders2D.Length == 0) { closestFlockAgent = null; return false; }
        Collider2D closestCollider2D = selectedColliders2D.FirstOrDefault(a => Vector3.Distance(transform.position, a.transform.position) < Range);
        if (closestCollider2D == null) { closestFlockAgent = null; return false; }
        closestFlockAgent = ships[closestCollider2D];
        return true;
    }
    private bool EnemiesInRange(out Vector3 averagePosition, out Collider2D[] selectedColliders2D, float Range)
    {
        selectedColliders2D = Physics2D.OverlapCircleAll(transform.position, Range, (LayerMask)~(1 << 6 + ((ITeam)this).Team));
        averagePosition = Vector3.zero;
        for (int i = 0; i < selectedColliders2D.Length; i++)
        {
            averagePosition += selectedColliders2D[i].transform.position;
        }
        averagePosition /= selectedColliders2D.Length;
        return selectedColliders2D.Length > 0;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}