using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
[RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    [Header("Player Speeds" + "\n")]
    [Range(0, 50)] public float BaseSpeed = 15;
    [Range(0, 1)] public float BaseAcceleration = 1;
    [Range(0, 150)] public float GrappleSpeed = 30;
    [Range(50, 150)] public float DashSpeed = 100;
    [Range(0, 100)] public float FlyingSpeed = 50;
    [Range(0, 50)] public float BoostPadSpeed = 15;
    [Header("Player Timers" + "\n")]
    [Range(0, 0.5f)] public float DashTime = 0.25f;
    [Range(0, 3f)] public float BoostDuration = 1f;
    [Range(0, 15)] public float FlyTime = 5;
    [Range(0, 2.5f)] public float FlyChargeWindow =1f;
    [Range(0, 15)] public float DashResetTime = 5;
    [Header("Jump Specifics" + "\n")]
    [Range(0, 25)] public float JumpHeight = 15;
    [Range(0, 2)] public float DistToGround = 1;
    public LayerMask WhatIsGround;

    [Header("Player Other" + "\n")]
    private float CurrentTopSpeed = 1;
    private WaitForSeconds DashTimer,FlyWindow,BoostTimer;

    [HideInInspector] public bool InAir = false, FlyCharged = false,GroundMode=true,Dashing=false,CanDash=true;
    [HideInInspector] public Rigidbody RB;
    public GameObject PlaneModel;
    private MeshRenderer meshRenderer;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * DistToGround);
    }
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        DashTimer = new WaitForSeconds(DashTime);
        FlyWindow = new WaitForSeconds(FlyChargeWindow);
        BoostTimer = new WaitForSeconds(BoostDuration);
        CurrentTopSpeed = BaseSpeed;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        PlaneModel.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
       //if (InAir)
       //    return;

        if (GroundMode&&!InAir)
            DoGroundMode();
        else if(!GroundMode) DoFlyMode();

        
    }
    private void DoFlyMode()
    {
        //enable plane model, disable player model
        meshRenderer.enabled = false;
        PlaneModel.gameObject.SetActive(true);
        PlaneModel.transform.localRotation = Quaternion.Euler(transform.GetChild(0).rotation.eulerAngles.x,0,0);
        CurrentTopSpeed = FlyingSpeed;
        RB.velocity += transform.GetChild(0).rotation* Vector3.forward;

        if (RB.velocity.magnitude > CurrentTopSpeed)
            RB.velocity = RB.velocity.normalized * CurrentTopSpeed;


         Invoke(nameof(ResetFly), FlyTime);
    }
    private void DoGroundMode()
    {
        Vector3 Vec = Vector3.zero + Physics.gravity * Time.deltaTime;
        Vec += transform.rotation * Vector3.right  * Input.GetAxisRaw("Horizontal");
        Vec += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical");
        float Yspeed = RB.velocity.y;
        RB.velocity += Vec * BaseAcceleration/2;
        RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);

        //drag 
        if (Input.GetAxis("Horizontal") == 0.0f&& Input.GetAxis("Vertical") == 0.0f )
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
            RB.velocity += JumpHeight * Vector3.up* Convert.ToInt32(GetIfGrounded());
        
        //dash
        if (Input.GetKeyDown(KeyCode.LeftShift)&& CanDash)
            StartCoroutine(Dash());

        //fly
        if (FlyCharged && Input.GetKeyDown(KeyCode.LeftControl))
            GroundMode = false;
    }
    public bool GetIfGrounded()
    {      
         return Physics.Raycast(transform.position, Vector3.down, DistToGround, WhatIsGround);     
    }

    private IEnumerator Dash()
    {
        CurrentTopSpeed = DashSpeed;
        Dashing = true;
        CanDash = false;
        Invoke(nameof(ResetDash), DashResetTime);
        //uncomment the commented parts if you want UD vel to be unaffected by gravity
        //float Yspeed = RB.velocity.y;
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed;
        //RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        yield return DashTimer;
        CurrentTopSpeed = BaseSpeed;
        Dashing = false;
    }
    private IEnumerator Boost(Vector3 Direction)
    {
        Debug.Log(Direction);
        CurrentTopSpeed = BoostPadSpeed;
        RB.velocity = Direction * CurrentTopSpeed;
        yield return BoostTimer;
        CurrentTopSpeed = BaseSpeed;
    }
    private IEnumerator FlyCharge()
    {
        FlyCharged = true;
        yield return FlyWindow;
        FlyCharged = false;
    }
   
    internal void ResetDash()
    {
        CanDash = true;
    }
    private void ResetFly()
    {
        meshRenderer.enabled = true;
        PlaneModel.gameObject.SetActive(false);
        CurrentTopSpeed = BaseSpeed;
        GroundMode = true;
    }
  

    public void ActivateCharge()
    {
        StartCoroutine(FlyCharge());
    }
    public void ActivateBoost(Vector3 Direction)
    {
       
        StartCoroutine(Boost(Direction));
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!collision.gameObject.TryGetComponent(out BasicEnemy Enemy))
    //        return;
    //
    //    Destroy(collision.gameObject);
    //    StartCoroutine(FlyCharge());
    //    
    //}
}
