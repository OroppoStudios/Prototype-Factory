using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class Missle : MonoBehaviour
{
    public bool Tracking;
    public Transform Target;
    public float Speed = 25.0f;
    private void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.right*Speed;
        if (!Tracking) return;
        GetComponent<Rigidbody>().velocity = Vector3.Normalize(Target.position - transform.position) * Speed;
        transform.LookAt(Target);
        transform.RotateAround(transform.position, transform.rotation * Vector3.up, -90);
   
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
