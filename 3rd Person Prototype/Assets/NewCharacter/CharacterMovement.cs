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
  
        Vector3 Vec = Vector3.zero + Physics.gravity*Time.deltaTime;
        Vec += transform.rotation * Vector3.right  * Input.GetAxisRaw("Horizontal");
        Vec += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical");
        float Yspeed = RB.velocity.y;

        if (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") == 0.0f )
        {
            if( RB.velocity.magnitude > 0.5f)
            RB.velocity = new Vector3(RB.velocity.x/10, Yspeed, RB.velocity.z/10);
            else RB.velocity = new Vector3(0, Yspeed, 0);
        }
        RB.velocity += Vec;
        RB.velocity = new Vector3(RB.velocity.x , Yspeed, RB.velocity.z );

        if (RB.velocity.magnitude> Speed)
        {
            //RB.velocity = RB.velocity.normalized * Speed;
            
            RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * Speed;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }

        //  if ((RB.velocity.z + RB.velocity.x) / 2 > Speed)
        //  {
        //      float Yspeed = RB.velocity.y;
        //      RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * Speed;
        //      RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        //  }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RB.velocity += 15.0f*Vector3.up;
        }
    }

    void Jump()
    {

    }
}
