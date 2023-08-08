using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
using System.Linq;

public class FlockAgent : MonoBehaviour, IShip, IDamagable, IWeapon
{
    public static Dictionary<Collider2D, FlockAgent> ships { get; private set; } = new Dictionary<Collider2D, FlockAgent>();
    public bool dogFighting = true;
    public Vector3 lookEndDirection;
    SpriteRenderer IShip.spriteRenderer { get; set; }
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
        ((IShip)this).spriteRenderer = GetComponent<SpriteRenderer>();
        if (Team != 1)
        {
            ((IShip)this).spriteRenderer.color = Color.magenta;
        }
    }
    private void OnDisable()
    {
        ships.Remove(GetComponent<Collider2D>());//memory leak fix
        if (PlayerUnitController.Singleton.selected.Contains(this))
        {
            PlayerUnitController.Singleton.selected.Remove(this);
        }
        if (PlayerUnitController.Singleton.finalSelected.Contains(this))
        {
            PlayerUnitController.Singleton.finalSelected.Remove(this);
        }
    }
    private void Start()
    {
        lookEndDirection = transform.up;
        targetDestination = transform.position;
    }
    private void Update()
    {
        IEnumerable<FlockAgent> shipsInRange = Physics2D.OverlapCircleAll(transform.position, Range)
            .Where(x => ships.ContainsKey(x) && ships[x] != this)
            .Select(y => ships[y]);
        if (NextAttackTime < Time.time)
        {
            Attack(shipsInRange.FirstOrDefault(x => (Vector3.Distance(transform.position, x.transform.position) < Range) && x.Team != Team));
        }
        if (dogFighting)
        {
            if (shipsInRange.Any(x => x.Team != Team))
            {
                DogFightVelocity(Range * 2, shipsInRange);
                MoveForward();
                return;
            }
        }
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * 3.0f);
        velocity = Vector2.Lerp(velocity, targetDestination - transform.position, blend);
        if (Vector3.Distance(targetDestination, transform.position) < 0.1f)
        {
            LookTowards(lookEndDirection);
            return;
        }
        MoveForward();
    }
    private void Attack(FlockAgent agent)
    {
        if (agent == null) { return; }
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
    private void DogFightVelocity(float Range, IEnumerable<FlockAgent> shipsInRange)
    {
        Vector2 positionMove =
            GameManager.Singleton.flockBehaviour.CalculateMove(
            this,
            shipsInRange.Where(x => x.Team == Team).ToArray(),
            shipsInRange.Where(x => x.Team != Team).ToArray()
            );
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime);
        velocity = Vector2.Lerp(velocity, positionMove.normalized * 3.0f, blend);
        velocity = velocity.normalized * Mathf.Lerp(velocity.magnitude, 3.0f, blend);
    }
    private void MoveForward()
    {
        velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, 0.0f, 4.0f);
        transform.up = velocity;
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
    private void LookTowards(Vector3 up)
    {
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, up), blend);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}