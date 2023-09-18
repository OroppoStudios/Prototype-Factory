using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthSystem))]

public class CharacterMovement : MonoBehaviour
{
    [Header("Player Speeds" + "\n")]
    [Range(0, 150)] public float BaseSpeed = 15;
    [Range(0, 5)] public float BaseAcceleration = 1;
    [Range(0, 3)] public float BaseDecceleration = 1;
    [Range(0, 3)] public float DashDecceleration = 1;
    [Range(0, 3)] public float BoostDecceleration = 1;
    [Range(0, 250)] public float GrappleSpeed = 30;
    [Range(50, 150)] public float DashSpeed = 100;
    [Range(50, 150)] public float GroundPoundSpeed = 100;
    [Range(0, 300)] public float FlyingSpeed = 50;
    [Range(0, 1)] public float AirControlReduction = 1;
    [Range(0, 150)] public float TerminalVelocity = 80;
    [Header("Player Timers" + "\n")]
    [Range(0, 0.5f)] public float DashTime = 0.25f;
    [Range(0, 3f)] public float BoostDuration = 1f;
    [Range(0, 15)] public float FlyTime = 5;
    [Range(0, 2.5f)] public float FlyChargeWindow = 1f;
    [Range(0, 15)] public float DashResetTime = 5;
    [Tooltip("Speeds depending on how many pads you hit consecutively")]
    [Range(0, 300)] public List<float> BoostPadSpeeds;
    [Header("Jump Specifics" + "\n")]
    [Range(0, 25)] public float JumpHeight = 15;
    [Range(0, 2)] public float DistToGround = 1;
    public LayerMask WhatIsGround;

    private Vector3 vecta, TrackedVelocity = Vector3.zero;
    [Header("Player Other" + "\n")]
    [HideInInspector] public float CurrentTopSpeed = 1, SlowCurrentSpeed =0.01f;
    private WaitForSeconds DashTimer, FlyWindow, BoostTimer;
    // I added this to prototype extending flight duration mid flight - Kai
    public bool FlyExtended = false;

    [HideInInspector] public bool InAir = false, FlyCharged = false, GroundMode = true, Dashing = false, CanDash = true;
    [HideInInspector] public Rigidbody RB;
    public GameObject PlaneModel;
    public VisualEffect SonicBoom_VFX;
    private MeshRenderer meshRenderer;
    private bool AwaitReset = false;
    [HideInInspector] public bool IsBoosting = false, IsDecellerating=false;
    [HideInInspector] public int BoostLevel = 0;

    public DeveloperConsoleBehaviour Console;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * DistToGround);

        Gizmos.DrawLine(transform.position, transform.position + vecta);
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

        PlayerInput.Move += RegularMovement;
        PlayerInput.Jump += Jump;
        PlayerInput.GroundPound += StartGroundPound;
        PlayerInput.Dash += StartDash;
        
    }
    private void OnDestroy()
    {
        PlayerInput.Move -= RegularMovement;
        PlayerInput.Jump -= Jump;
        PlayerInput.GroundPound -= StartGroundPound;
        PlayerInput.Dash -= StartDash;
    }
    // Update is called once per frame
    void Update()
    {
        //Developer Console
      // if (Input.GetKeyDown(KeyCode.Return))
      //     Console.OnToggle();
        //if (InAir)
        //    return;

        //use this if you dont want to be able to control while in air
        //&&GetIfGrounded()
     //if (GroundMode && !InAir)
     //    DoGroundMode();
     // else if (!GroundMode) DoFlyMode();

        if (AwaitReset && GetIfGrounded())
        {
            CanDash = true;
            AwaitReset = false;
        }
        //lazy fix
        //basically dash wouldnt trigger cause it was in the do ground mode func
        //jump


        //dash
       
    }

    private void DoFlyMode()
    {
        //enable plane model, disable player model
        meshRenderer.enabled = false;
        PlaneModel.gameObject.SetActive(true);
        PlaneModel.transform.localRotation = Quaternion.Euler(transform.GetChild(0).rotation.eulerAngles.x, 0, 0);
        CurrentTopSpeed = FlyingSpeed ;
        RB.velocity = transform.GetChild(0).rotation * Vector3.forward* CurrentTopSpeed;

        if (RB.velocity.magnitude > CurrentTopSpeed / 3.6f)
            RB.velocity = RB.velocity.normalized * CurrentTopSpeed / 3.6f;

        Invoke(nameof(ResetFly), FlyTime);

        // I added this to prototype extending flight duration mid flight - Kai
        if (FlyExtended == true)
        {
            CancelInvoke(nameof(ResetFly));
            Invoke(nameof(ResetFly), FlyTime);
            FlyExtended = false;
        }
    }
    private void RegularMovement(Vector2 InputVec)
    {
     
        Vector3 Vec = Vector3.zero + Physics.gravity * Time.deltaTime;
        Vec += transform.rotation * Vector3.right * InputVec.x;
        Vec += transform.rotation * Vector3.forward * InputVec.y;

        //reduce control in air
        if (!GetIfGrounded())
            Vec *= (1 - AirControlReduction);

        float Yspeed = RB.velocity.y;

       

        RB.velocity += Vec *(BaseAcceleration) ;
        RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
       

        if (InputVec == Vector2.zero)
        {
         //  if (RB.velocity.magnitude > 0.5f)
         //      RB.velocity = new Vector3((RB.velocity.x - RB.velocity.x / BaseDecceleration), Yspeed, (RB.velocity.z - RB.velocity.z / BaseDecceleration));
         //  else RB.velocity = new Vector3(0, Yspeed, 0);
         //

            if (IsDecellerating == false) TrackedVelocity = RB.velocity;

            SlowDown(Yspeed);
            IsDecellerating = true;
        }
        else
        {
            IsDecellerating = false;
            SlowCurrentSpeed = 0.01f;
        }



        if (RB.velocity.magnitude > CurrentTopSpeed / 3.6f)
        {
            //RB.velocity = RB.velocity.normalized * Speed;

            RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed / 3.6f;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }



        TerminalVelocityCalc();
    }
    private void Jump()
    {
        Debug.Log("jump "+ Convert.ToInt32(GetIfGrounded()));
        RB.velocity += JumpHeight * Vector3.up * Convert.ToInt32(GetIfGrounded());
    }
    private void DoGroundMode()
    {
        Vector3 Vec = Vector3.zero + Physics.gravity * Time.deltaTime;
     //  Vec += transform.rotation * Vector3.right * Input.GetAxisRaw("Horizontal");
     //  Vec += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical");

        //reduce control in air
        if (!GetIfGrounded())
            Vec *= (1 - AirControlReduction);

       //if (Input.GetKeyDown(KeyCode.Space))
       //    RB.velocity += JumpHeight * Vector3.up * Convert.ToInt32(GetIfGrounded());
       //
       //if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash)
       //    StartDash();
       //
       ////GroundPound
       //if (Input.GetKeyDown(KeyCode.Q))
       //    StartGroundPound();
       //
       ////fly
       //if (FlyCharged && Input.GetKeyDown(KeyCode.LeftControl))
       //    GroundMode = false;

        vecta = Vec;
        float Yspeed = RB.velocity.y;

        if (!((Physics.Raycast(transform.position, transform.rotation * Vector3.forward, 1, WhatIsGround) && !GetIfGrounded())))
        {
            RB.velocity += Vec * BaseAcceleration / 2;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }


        //drag 
      // if (Input.GetAxis("Horizontal") == 0.0f && Input.GetAxis("Vertical") == 0.0f)
      // {
      //     //  if (RB.velocity.magnitude > 0.5f)
      //     //      RB.velocity = new Vector3((RB.velocity.x - RB.velocity.x / BaseDecceleration), Yspeed, (RB.velocity.z - RB.velocity.z / BaseDecceleration));
      //     //  else RB.velocity = new Vector3(0, Yspeed, 0);
      //     if (IsDecellerating==false) TrackedVelocity = RB.velocity;
      //
      //     SlowDown(Yspeed);
      //     IsDecellerating = true;
      // }
      // else
      // {
      //     IsDecellerating = false;
      //     SlowCurrentSpeed = 0.01f;
      // }

      // else if 
      //     RB.velocity += transform.rotation * Vector3.back;

        //speed cap
        if (RB.velocity.magnitude > CurrentTopSpeed / 3.6f)
        {
            //RB.velocity = RB.velocity.normalized * Speed;

            RB.velocity += new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed / 3.6f;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }
      
       

    }
    private void SpeedUp()
    {

    }
    private void SlowDown(float YVel)
    {
       // Debug.Log("slow " +IsDecellerating + " " + SlowCurrentSpeed + " " + BaseDecceleration);

        if (!IsDecellerating||SlowCurrentSpeed> BaseDecceleration) return;

          if (RB.velocity.magnitude > 2f)
                RB.velocity = new Vector3((TrackedVelocity.x * (1 - (SlowCurrentSpeed / BaseDecceleration))), YVel, (TrackedVelocity.z  *(1 - (SlowCurrentSpeed / BaseDecceleration))));
            else RB.velocity = new Vector3(0, YVel, 0);

            SlowCurrentSpeed += Time.deltaTime;

        IsDecellerating = false;
        CurrentTopSpeed = BaseSpeed;
    }
    public void TerminalVelocityCalc()
    {
        //basically a Y velocity clamp
        RB.velocity = new Vector3(RB.velocity.x, Mathf.Clamp(RB.velocity.y,
            -TerminalVelocity / 3.6f, 500), RB.velocity.z);
    }

    private void StartGroundPound()
    {
        //this is so that other speed boosts do not stop the pound short and reset its trajectory
        StopAllCoroutines();
        StartCoroutine(GroundPound());
        SonicBoom_VFX.Play();
    }
    private IEnumerator GroundPound()
    {
        Debug.Log("Doing Ground Pound");
        yield return null;
    }

    #region DashAbility
 
    private void StartDash()
    {
        //this is so that other speed boosts do not stop the dash short and reset its trajectory
        StopAllCoroutines();
        StartCoroutine(Dash());
        SonicBoom_VFX.Play();
    }
    private void ResetDash()
    {
        AwaitReset = true;
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
        if (RB.velocity.magnitude <= 0.1f)
            RB.velocity = transform.rotation * Vector3.forward * CurrentTopSpeed;
        //RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        yield return DashTimer;
        Dashing = false;
        StartCoroutine(DashSlowDown());
    }
    private IEnumerator DashSlowDown()
    {
        float i = 0.01f;
        while (i < DashDecceleration)
        {
            CurrentTopSpeed = (BaseSpeed + (DashSpeed - BaseSpeed) * (1 - i / DashDecceleration));
            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        CurrentTopSpeed = BaseSpeed;
    }


    #endregion DashAbility

    #region BoostAbility
    private IEnumerator Boost(Vector3 Direction, float Speed)
    {
        IsBoosting = true;
        CurrentTopSpeed = Speed;
        RB.velocity = Direction * CurrentTopSpeed/3.6f;
        yield return BoostTimer;
        StartCoroutine(BoostSlowDown());
        IsBoosting = false;
        BoostLevel = 0;
    }
    private IEnumerator BoostSlowDown()
    {
        float i = 0.01f;
        float BoostedSpeed = RB.velocity.magnitude;
        while (i < BoostDecceleration)
        {
            CurrentTopSpeed = (BaseSpeed + (BoostedSpeed*3.6f - BaseSpeed) * (1 - i / BoostDecceleration));
            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        CurrentTopSpeed = BaseSpeed;
    }
    #endregion BoostAbility
    private IEnumerator FlyCharge()
    {
        FlyCharged = true;
        yield return FlyWindow;
        FlyCharged = false;
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
    public bool GetIfGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, DistToGround, WhatIsGround);
    }
    public void ActivateBoost(Vector3 Direction)
    {
        if (BoostLevel != 0)
            StopAllCoroutines();

        StartCoroutine(Boost(Direction, BoostPadSpeeds[BoostLevel]));
        SonicBoom_VFX.Play();

        if (BoostLevel < BoostPadSpeeds.Count)
            BoostLevel++;

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
