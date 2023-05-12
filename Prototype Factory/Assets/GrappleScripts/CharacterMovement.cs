using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    [Range(0, 50)] public float Speed = 15;
    [HideInInspector] public bool InAir = false;
    [HideInInspector] public Rigidbody RB;
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (InAir)
            return;
  
        Vector3 Vec = Vector3.zero;
        Vec += transform.rotation * Vector3.right  * Input.GetAxisRaw("Horizontal");
        Vec += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical");
        RB.velocity += Vec;
        if (RB.velocity.magnitude > Speed)
            RB.velocity = RB.velocity.normalized * Speed;
    }
}
