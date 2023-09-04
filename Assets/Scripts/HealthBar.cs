using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float health = 1.0f;
    private Color teamColour = Color.white;
    private SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void ManualUpdate()
    {
        health = Mathf.Clamp01(health);
        spriteRenderer.transform.localPosition = new Vector3(0.45f * (health - 1), spriteRenderer.transform.localPosition.y, spriteRenderer.transform.localPosition.z);
        spriteRenderer.transform.localScale = new Vector3(0.9f * health, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
    }
    public void Set(float health)
    {
        this.health = health;
        ManualUpdate();
    }
    /*
     * WTF WHY IS UPDATE NOT WORKING
     * IT JUST IS NOT CALLING UPDATE ON AN ENABLED SCRIPT
     * I HAVE JSUT SPENT 3 HOURS TRYING TO FIX THIS
     * WHY UNITY WHY??????
     */
}
