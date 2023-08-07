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
    SpriteRenderer ISelectable.spriteRenderer { get; set; }
    [SerializeField] private int _team = 0;
    [System.NonSerialized] public Vector3 targetDestination;
    private Vector2 velocity = Vector2.zero;
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
            Collider2D[] selectedColliders2D = Physics2D.OverlapCircleAll(transform.position, Range);
            if (selectedColliders2D.Length > 1)
            {
                DogFight(Range * 2, selectedColliders2D);
                MoveForward();
            }
            else
            {
                float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime);
                velocity = Vector2.Lerp(velocity, Vector2.zero, blend);
                MoveForward();
            }
        }
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
    private void DogFight(float Range, Collider2D[] colliders2D)
    {
        IEnumerable<IGrouping<int, FlockAgent>> shipsSorted = colliders2D
            .Where(x => ships.ContainsKey(x))
            .Select(y => ships[y])
            .GroupBy(a => a.Team);
        Vector2 avoidancePosition = Vector2.zero;
        Vector2 cohesionUp = Vector2.zero;
        Vector2 chasePosition = Vector2.zero;
        int friendlyCount = shipsSorted.Where(a => a.Key == Team).Count();
        int enemyCount = shipsSorted.Count() - friendlyCount;
        foreach (IGrouping<int, FlockAgent> team in shipsSorted)
        {
            foreach (FlockAgent ship in team)
            {
                if (ship == this) { continue; }
                avoidancePosition += ((Vector2)ship.transform.position / (friendlyCount + enemyCount)) * Mathf.Pow(0.99f, Vector2.Distance((Vector2)transform.position, (Vector2)ship.transform.position));// * 50f;
                if (ship.Team == Team)
                {
                    cohesionUp += (Vector2)ship.transform.up / friendlyCount;
                }
                else
                {
                    chasePosition += (Vector2)ship.transform.position / enemyCount;
                }
            }
        }
        cohesionUp.Normalize();
        Vector2 finalPositionMove = chasePosition - avoidancePosition;
        //finalPositionMove += cohesionUp;
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime);
        velocity = Vector2.Lerp(velocity, finalPositionMove - (Vector2)transform.position, blend);
        if (Team != 1)
        {
            Debug.Log(finalPositionMove);
        }
    }
    private void MoveForward()
    {
        transform.up = velocity;
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}