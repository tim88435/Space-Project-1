using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float finalSize = 4f;
    private void Start()
    {
        Destroy(gameObject, 2f);
    }
    private void Update()
    {
        if (transform.parent != null)
        {
            if (transform.parent.lossyScale.magnitude == 0)
            {
                return;
            }
        }
        float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * 0.5f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * finalSize, blend);
    }
}