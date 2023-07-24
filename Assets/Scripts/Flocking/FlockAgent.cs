using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Interfaces;

public class FlockAgent : MonoBehaviour, ITeam, ISelectable
{
    public static Dictionary<Collider2D, FlockAgent> agents { get; private set; } = new Dictionary<Collider2D, FlockAgent>();
    public Vector3 lookEndDirection = Vector3.zero;
    [SerializeField] int teamID = 0;
    SpriteRenderer ISelectable.spriteRenderer { get; set; }
    int ITeam.Team
    {
        get
        {
            return teamID;
        }
        set
        {
            teamID = value;
        }
    }
    [System.NonSerialized] public Vector3 targetDestination = Vector3.zero;
    private void OnEnable()
    {
        agents.Add(GetComponent<Collider2D>(), this);
        ((ISelectable)this).spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnDisable()
    {
        agents.Remove(GetComponent<Collider2D>());//memory leak fix
    }
    private void Start()
    {
        targetDestination = transform.position;
    }
    private void Update()
    {
        float distance = Vector2.Distance(transform.position, targetDestination);
        if (distance > 0.1f)
        {
            MoveForward(distance);
            RotateTowards(targetDestination);
        }
        else
        {
            RotateTowards(lookEndDirection);
        }
    }
    private void MoveForward(float distance)
    {
        transform.Translate(Vector2.up * Time.deltaTime * Mathf.Min(distance * 7.0f, 5.0f));
    }
    private void RotateTowards(Vector3 location)
    {
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * 10.0f);
        Quaternion rotation = Quaternion.LookRotation(transform.forward, location - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, blend);
    }
}