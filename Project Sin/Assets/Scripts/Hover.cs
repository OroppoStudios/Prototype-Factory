using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float speed = 1.5f;
    public float StartHeight;

    private void Start()
    {
        StartHeight = transform.position.y;
    }
    void FixedUpdate()
    {
        Vector3 p = transform.position;
        p.y = amplitude * Mathf.Cos(Time.time * speed) + StartHeight;
        transform.position = p;
    }
}
