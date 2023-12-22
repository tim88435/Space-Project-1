using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;
using System.Linq;
[SelectionBase]
public class FlockAgent : MonoBehaviour, IColourable, IDamagable, IWeapon
{
    public static Dictionary<Collider2D, FlockAgent> ships { get; private set; } = new Dictionary<Collider2D, FlockAgent>();
    public bool dogFighting = true;
    public Vector3 lookEndDirection;
    SpriteRenderer IColourable.SpriteRenderer { get; set; }
    [SerializeField] private int _team = 0;
    [System.NonSerialized] public Vector3 targetDestination = Vector3.back;
    private Vector2 velocity = Vector2.zero;
    private HealthBar healthBar;
    public int TeamID
    {
        get { return _team; }
        set
        {
            _team = value;
            gameObject.layer = 6 + ((ITeam)this).TeamID;
        }
    }
    public float Health { get; set; } = 10f;
    public float MaxHealth { get; set; } = 10f;
    public float Evasion { get; set; } = 0.3f;
    public float DamagePerHit { get; set; } = 1;
    public float CooldownSeconds { get; set; } = 3;
    public float NextAttackTime { get; set; } = 0;
    public float Range { get; set; } = 15f;

    private void OnEnable()
    {
        TeamID = TeamID;//set layer
        ships.Add(GetComponent<Collider2D>(), this);
        ((IColourable)this).SpriteRenderer = GetComponent<SpriteRenderer>();
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
        ((IColourable)this).SetColour(GameManager.Singleton.teamColours[TeamID]);
        lookEndDirection = transform.up;
        if (targetDestination == Vector3.back)
        {
            targetDestination = transform.position;
        }
    }
    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.Set(Health / MaxHealth);
        }
        IEnumerable<FlockAgent> shipsInRange = Physics2D.OverlapCircleAll(transform.position, Range)
            .Where(x => ships.ContainsKey(x) && ships[x] != this)
            .Select(y => ships[y]);
        if (NextAttackTime < Time.time)
        {
            AttackAgent(shipsInRange.FirstOrDefault(x => (Vector3.Distance(transform.position, x.transform.position) < Range) && x.TeamID != TeamID));
        }
        if (dogFighting)
        {
            if (shipsInRange.Any(x => x.TeamID != TeamID))
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
    private void AttackAgent(FlockAgent agent)
    {
        if (agent == null) { return; }
        Attack(agent, agent.transform.position);
    }
    private void Attack(IDamagable damagable, Vector3 hitPosition)
    {
        if (damagable == null) { return; }
        if (damagable.TryHitWith(this))
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
        lineRenderer.sortingOrder = 1;
        Destroy(lineRenderer.gameObject, 0.5f);
    }
    private void DogFightVelocity(float Range, IEnumerable<FlockAgent> shipsInRange)
    {
        Vector2 positionMove =
            GameManager.Singleton.flockBehaviour.CalculateMove(
            this,
            shipsInRange.Where(x => x.TeamID == TeamID).ToArray(),
            shipsInRange.Where(x => x.TeamID != TeamID).ToArray()
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
    public void ShowHealthBar(bool shouldShow)
    {
        if (shouldShow)
        {
            if (healthBar == null)
            {
                healthBar = Instantiate(GameManager.prefabList.healthBarPrefab, transform).GetComponent<HealthBar>();
            }
            healthBar.transform.up = Vector3.up;
            healthBar.gameObject.SetActive(true);
            return;
        }
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}