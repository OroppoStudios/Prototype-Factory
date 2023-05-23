using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    [Range(0, 50)] public float BaseSpeed = 15;
    [Range(0, 150)] public float GrappleSpeed = 30;
    [Range(50, 150)] public float DashSpeed = 100;
    [Range(100, 300)] public float FlyingSpeed = 200;

    [Range(0, 0.5f)] public float DashTime = 0.25f;
    [Range(0, 30)] public float FlyTime = 15;
    [Range(0, 2.5f)] public float FlyChargeWindow =1f;
    private float CurrentTopSpeed = 1;
    private WaitForSeconds DashTimer,FlyWindow;
    [Range(0, 25)] public float JumpHeight = 15;
    [HideInInspector] public bool InAir = false, FlyCharged = false,GroundMode=true;
    [HideInInspector] public Rigidbody RB;
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        DashTimer = new WaitForSeconds(DashTime);
        FlyWindow = new WaitForSeconds(FlyChargeWindow);
        CurrentTopSpeed = BaseSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        if (InAir)
            return;

        if (GroundMode)
            DoGroundMode();
        else DoFlyMode();

        
    }
    private void DoFlyMode()
    {
        Invoke(nameof(FlyModeOver), FlyTime);
    }
    private void DoGroundMode()
    {
          //basic Inputs
        Vector3 Vec = Vector3.zero + Physics.gravity*Time.deltaTime;
        Vec += transform.rotation * Vector3.right  * Input.GetAxisRaw("Horizontal");
        Vec += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical");
        float Yspeed = RB.velocity.y;


        RB.velocity += Vec;
        RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);

        //drag 
        if (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") == 0.0f )
        {
            if( RB.velocity.magnitude > 0.5f)
            RB.velocity = new Vector3(RB.velocity.x/10, Yspeed, RB.velocity.z/10);
            else RB.velocity = new Vector3(0, Yspeed, 0);
        }

        //speed cap
        if (RB.velocity.magnitude> CurrentTopSpeed)
        {
            //RB.velocity = RB.velocity.normalized * Speed;
            
            RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }
        //jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RB.velocity += JumpHeight * Vector3.up;
        }

        //dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartCoroutine(Dash());

        if (FlyCharged && Input.GetKeyDown(KeyCode.LeftControl))
            GroundMode = false;
    }
    private IEnumerator Dash()
    {
        CurrentTopSpeed = DashSpeed;

        //uncomment the commented parts if you want UD vel to be unaffected by gravity
      //  float Yspeed = RB.velocity.y;
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed;
     //   RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        yield return DashTimer;
        CurrentTopSpeed = BaseSpeed;
    }
    private IEnumerator FlyCharge()
    {
        FlyCharged=true;
        yield return FlyWindow;
        FlyCharged = false;
    }
    private void FlyModeOver()
    {
        GroundMode = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out BasicEnemy Enemy))
            return;

        Destroy(collision.gameObject);
        StartCoroutine(FlyCharge());
        
    }
}
