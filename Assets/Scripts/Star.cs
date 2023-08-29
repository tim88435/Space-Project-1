using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Star : MonoBehaviour
{
    [SerializeField] AnimationCurve lifetimeSize = new AnimationCurve();
    [SerializeField] Gradient lifetimeColour = new Gradient();
    private float scale = 10000.0f;
    [SerializeField] private Material outlineMaterial;
    private SpriteRenderer[] children = new SpriteRenderer[16];
    private void Start()
    {
        children[0] = transform.GetComponentInChildren<SpriteRenderer>(); ;
        for (int i = 0; i < children.Length; i++)
        {
            if (i == 0) continue;
            children[i] = new GameObject().AddComponent<SpriteRenderer>();
            children[i].transform.position = transform.position;
            children[i].transform.parent = transform;
            children[i].material = outlineMaterial;
            children[i].color = children[0].color;
            children[i].sprite = children[0].sprite;
            children[i].transform.localScale = Vector3.zero;
        }
    }
    private void Update()
    {
        //          (Time.time + 590)
        float time = Time.time * (1.0f / GameManager.GameLengthSeconds);
        UpdateSizes(time);
        children[0].color = lifetimeColour.Evaluate(time);
        transform.localScale = Vector3.one * scale * lifetimeSize.Evaluate(time);
        FlockAgent[] flockAgents = FlockAgent.ships.Values.ToArray();
        for (int i = 0; i < flockAgents.Length; i++)
        {
            float distance = Vector3.Distance(flockAgents[i].transform.position, transform.position) - transform.lossyScale.x * 0.5f;
            if (distance > 0)
            {
                continue;
            }
            ((IDamagable)flockAgents[i]).DealDirectDamage(-distance * Time.deltaTime);
        }
    }
    private void UpdateSizes(float time)
    {
        if (time < 1)
        {
            children[0].transform.localScale = Vector3.one;
            for (int i = 0; i < children.Length; i++)
            {
                if (i == 0) continue;
                children[i].transform.localScale = Vector3.zero;
            }
            return;
        }
        children[0].transform.localScale = Vector3.one * (1 / lifetimeSize.Evaluate(time)) * lifetimeSize.Evaluate(1);
        for (int i = 0; i < children.Length; i++)
        {
            if (i == 0) continue;
            float blend = 1 - Mathf.Pow(0.5f, Time.timeScale * 0.1f * 1 / i);
            Vector3 finalScale = Vector3.one * (((float)i + 2) / children.Length);
            children[i].transform.localScale = Vector3.Lerp(children[i].transform.localScale, finalScale, blend);
        }
    }
}
