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
    #region Stats
    [Header("Player Speeds" + "\n")]
    [Range(0, 150)] public float BaseSpeed = 15;
    [Range(0, 5)] public float BaseAcceleration = 1;
    [Range(0, 3)] public float BaseDecceleration = 1;
    [Range(0, 3)] public float DashDecceleration = 1;
    [Range(0, 3)] public float BoostDecceleration = 1;
    [Range(0, 250)] public float GrappleSpeed = 30;
    [Range(50, 150)] public float DashSpeed = 100;
    [Range(50, 200)] public float GroundPoundSpeed = 100;
    [Range(0, 300)] public float FlyingSpeed = 50;
    [Range(0, 1)] public float AirControlReduction = 1;
    [Range(0, 150)] public float TerminalVelocity = 80;

    [Header("Player Timers" + "\n")]
    [Range(0, 0.5f)] public float DashTime = 0.25f;
    [Range(0, 3f)] public float BoostDuration = 1f;
    [Range(0, 15)] public float FlyTime = 5;
    [Range(0, 2.5f)] public float FlyChargeWindow = 1f;
    [Range(0, 15)] public float DashResetTime = 5;
    [Range(0, 10)] public float GroundPoundCooldownTime = 3;

    [Tooltip("Speeds depending on how many pads you hit consecutively")]
    [Range(0, 300)] public List<float> BoostPadSpeeds;

    [Header("Jump Statistics" + "\n")]
    [Range(0, 25)] public float JumpHeight = 15;
    [Range(0, 2)] public float DistToGround = 1;
    [HideInInspector] public bool IsBoosting = false, IsDecellerating = false;
    [HideInInspector] public int BoostLevel = 0;

    [Header("Player Other" + "\n")]
    [HideInInspector] public float CurrentTopSpeed = 1, SlowCurrentSpeed = 0.01f;
    private WaitForSeconds DashTimer, FlyWindow, BoostTimer;

    // I added this to prototype extending flight duration mid flight - Kai
    public bool FlyExtended = false;
    private bool AwaitReset = false;
    [HideInInspector] public bool playerInAir = false, FlyCharged = false, GroundMode = true, Dashing = false, CanDash = true, CanGroundPound = true, grounded =true;
    private Vector3 vecta, TrackedVelocity = Vector3.zero;
    public LayerMask WhatIsGround;
    private bool _jumpPressed;
    private bool _landed;
    private bool _attacked;

    [Header("Animation Timings \n")]
    [SerializeField]
    private float _attackAnimTime;

    [SerializeField] 
    private float _landAnimDuration;

    [SerializeField]
    private float animLockout = 0.2f;

    #endregion

    #region Cached Values
    [Header("Cached Values \n")]
    [HideInInspector] public Rigidbody RB;
    public GameObject PlaneModel;
    public VisualEffect SonicBoom_VFX;
    private Renderer meshRenderer;
    public DeveloperConsoleBehaviour Console;
    public MovementState PlayersState;
    private Animator anim;

    private int _currentState;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Land = Animator.StringToHash("Land");
    private static readonly int Attack = Animator.StringToHash("Attack"); //Unimplemented
    private static readonly int Collect = Animator.StringToHash("Collect"); //Unimplemented
    private static readonly int GroundPound = Animator.StringToHash("Ground Pound"); //Unimplemented
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * DistToGround);
        Gizmos.DrawLine(transform.position, transform.position + vecta);
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<Renderer>();
        anim = GetComponent<Animator>();

        DashTimer = new WaitForSeconds(DashTime);
        FlyWindow = new WaitForSeconds(FlyChargeWindow);
        BoostTimer = new WaitForSeconds(BoostDuration);
        CurrentTopSpeed = BaseSpeed;
        meshRenderer.enabled = true;
        CanGroundPound = true;
        PlayersState = new MovementState(this);
        PlayersState.ChangeState(MoveState.Standard);
        PlaneModel.gameObject.SetActive(false);

        PlayerInput.Jump += DoJump;
        PlayerInput.GroundPound += StartGroundPound;
        PlayerInput.Dash += StartDash;
        PlayerInput.FlyMode += StartFly;


    }

    private void OnDestroy()
    {
        PlayerInput.Move -= RegularMovement;
        PlayerInput.Jump -= DoJump;
        PlayerInput.GroundPound -= StartGroundPound;
        PlayerInput.Dash -= StartDash;
        PlayerInput.FlyMode -= StartFly;
    }

    void Update()
    {
        var state = GetAnimationState();

        _jumpPressed = false;
        _landed = false;
        _attacked = false;

        if (state == _currentState) return;
        anim.CrossFade(state, 0, 0);
        _currentState = state;

        if (AwaitReset && GetIfGrounded())
        {
            CanDash = true;
            AwaitReset = false;
        }
    }

    private int GetAnimationState()
    {
        if (Time.time < animLockout) return _currentState;

        // Priorities
        //if (_attacked) return LockState(Attack, _attackAnimTime);
        if (_landed) return LockState(Land, _landAnimDuration);
        if (_jumpPressed) return Jump;

       // Debug.Log("Velocity X:" + RB.velocity.x + " Velocity Z:" + RB.velocity.z);

        if (grounded) return Mathf.Abs(RB.velocity.x + RB.velocity.z) <= 0.3 ? Idle : Run;

        return RB.velocity.y > 0 ? Jump : Fall;

        int LockState(int s, float t)
        {
            animLockout = Time.time + t;
            return s;
        }
    }

    #region BasicMovement
    public void RegularMovement(Vector2 InputVec)
    {

        Vector3 Vec = Vector3.zero + Physics.gravity * Time.deltaTime;
        Vec += transform.rotation * Vector3.right * InputVec.x;
        Vec += transform.rotation * Vector3.forward * InputVec.y;

        //reduce control in air
        if (!GetIfGrounded())
            Vec *= (1 - AirControlReduction);

        float Yspeed = RB.velocity.y;
        RB.velocity += Vec * (BaseAcceleration);
        RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);

        if (InputVec == Vector2.zero)
        {
            //  if (RB.velocity.magnitude > 0.5f)
            //      RB.velocity = new Vector3((RB.velocity.x - RB.velocity.x / BaseDecceleration), Yspeed, (RB.velocity.z - RB.velocity.z / BaseDecceleration));
            //  else RB.velocity = new Vector3(0, Yspeed, 0);

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
            RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed / 3.6f;
            RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        }
        TerminalVelocityCalc();
    }
    private void DoJump()
    {     
        RB.velocity += JumpHeight * Vector3.up * Convert.ToInt32(GetIfGrounded());
    }

    private void SpeedUp()
    {

    }
    private void SlowDown(float YVel)
    {
        // Debug.Log("slow " +IsDecellerating + " " + SlowCurrentSpeed + " " + BaseDecceleration);

        // Debug.Log("try decellerating");
        if (!IsDecellerating || SlowCurrentSpeed > BaseDecceleration) return;

        //  Debug.Log("decellerating");

        if (RB.velocity.magnitude > 3f)
            RB.velocity = new Vector3((TrackedVelocity.x * (1 - (SlowCurrentSpeed / BaseDecceleration))), YVel, (TrackedVelocity.z * (1 - (SlowCurrentSpeed / BaseDecceleration))));
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
    #endregion BasicMovement

    #region GroundPoundAbility
    private void StartGroundPound()
    {
        Debug.Log(CanGroundPound);
       if (!CanGroundPound)
           return;

        CanGroundPound = false;
        //this is so that other speed boosts do not stop the pound short and reset its trajectory
        StopAllCoroutines();
        PlayersState.ChangeState(MoveState.GroundPounding);
        SonicBoom_VFX.Play();
    }
 
    public void GroundPoundMovement(Vector2 vec)
    {
        RB.velocity = transform.rotation * Vector3.down * GroundPoundSpeed;
        if (GetIfGrounded())
        {
            PlayersState.ChangeState(MoveState.Standard);
            Invoke(nameof(ResetGroundPound), GroundPoundCooldownTime);
        }
          
    }

    void ResetGroundPound()
    {
        CanGroundPound = true; 
    }
    #endregion GroundPoundAbility

    #region DashAbility

    private void StartDash()
    {
        if (!CanDash)
            return;
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
        if (RB.velocity.magnitude <= 0.1f)
            RB.velocity = transform.rotation * Vector3.forward * CurrentTopSpeed;
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z).normalized * CurrentTopSpeed;
        
        
        //RB.velocity = new Vector3(RB.velocity.x, Yspeed, RB.velocity.z);
        yield return DashTimer;
        Dashing = false;
        IsDecellerating = false;
        SlowCurrentSpeed = 0.01f;
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
        RB.velocity = Direction * CurrentTopSpeed / 3.6f;
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
            CurrentTopSpeed = (BaseSpeed + (BoostedSpeed * 3.6f - BaseSpeed) * (1 - i / BoostDecceleration));
            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        CurrentTopSpeed = BaseSpeed;
    }
    #endregion BoostAbility

    #region FlyAbility
    private IEnumerator FlyCharge()
    {
        FlyCharged = true;
        yield return FlyWindow;
        FlyCharged = false;
    }


    private void ResetFly()
    {
        PlayersState.ChangeState(MoveState.Standard);
        meshRenderer.enabled = true;
        PlaneModel.gameObject.SetActive(false);
        CurrentTopSpeed = BaseSpeed;
        GroundMode = true;
    }
    public void StartFly()
    {
        if (!FlyCharged)
            return;

        PlayersState.ChangeState(MoveState.Flying);
    }
    public void FlyMovement(Vector2 InputVec)
    {
        //enable plane model, disable player model
        meshRenderer.enabled = false;
        PlaneModel.gameObject.SetActive(true);
        PlaneModel.transform.localRotation = Quaternion.Euler(transform.GetChild(0).rotation.eulerAngles.x, 0, 0);
        CurrentTopSpeed = FlyingSpeed;
        RB.velocity = transform.GetChild(0).rotation * Vector3.forward * CurrentTopSpeed;

        if (RB.velocity.magnitude > CurrentTopSpeed / 3.6f)
            RB.velocity = RB.velocity.normalized * CurrentTopSpeed / 3.6f;
        
        if (FlyCharged)
            Invoke(nameof(ResetFly), FlyTime);

        FlyCharged = false;
        // I added this to prototype extending flight duration mid flight - Kai
        if (FlyExtended == true)
        {
            CancelInvoke(nameof(ResetFly));
            Invoke(nameof(ResetFly), FlyTime);
            FlyExtended = false;
        }
    }

    public void ActivateCharge()
    {
        StartCoroutine(FlyCharge());
    }

    #endregion FlyAbility
    public bool GetIfGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, DistToGround, WhatIsGround)   ) 
            grounded = true;
        else
            grounded = false;

        return grounded;
    }
    public void ActivateBoost(Vector3 Direction)
    {
        if (BoostLevel != 0)
            StopAllCoroutines();

        StartCoroutine(Boost(Direction, BoostPadSpeeds[BoostLevel]));
        SonicBoom_VFX.Play();

        if (BoostLevel < BoostPadSpeeds.Count)
            BoostLevel++;
        IsDecellerating = false;
        SlowCurrentSpeed = 0.01f;

    }

}
