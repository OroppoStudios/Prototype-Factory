using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    [Range(0, 25)] public float Speed = 1;
    CharacterController Controller;
    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        Controller.Move(transform.rotation * Vector3.right * Speed * Input.GetAxisRaw("Horizontal")/100);
        Controller.Move(transform.rotation * Vector3.forward * Speed * Input.GetAxisRaw("Vertical")/100);
       
    }
}
