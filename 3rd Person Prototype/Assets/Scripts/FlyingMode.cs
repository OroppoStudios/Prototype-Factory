using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyingMode : MonoBehaviour
{

    private NewDash DashScript;
    private ThirdPersonController TPC;
    private CharacterController CC;
    private Rigidbody rb;

    public GameObject PlayerCharacter;
    public GameObject PlayerPlane;

    private PlayerInput _PInput;
    private StarterAssetsInputs _input;

    //// Input Values
    //private float thrust1D;
    //private float upDown1D;
    //private float strafe1D;
    //private float roll1D;
    //private Vector2 pitchYaw;



    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5.0f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;

    [Header("Flying Movement Settings")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 100f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    private float glide, verticalGlide, horizontalGlide = 0f;



    public bool FlyingModeState = false;
    private bool FlyingActionMapEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        DashScript = GetComponent<NewDash>();
        TPC = GetComponent<ThirdPersonController>();
        CC = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        

        _input = GetComponent<StarterAssetsInputs>();
        _PInput = GetComponent<PlayerInput>();
        
        PlayerPlane.SetActive(false);

       // StartFlying();
    }

    //public void OnMove(InputAction.CallbackContext context)
    //{
    //    Vector2 MoveAxis = context.ReadValue<Vector2>();
    //
    //    if (MoveAxis.sqrMagnitude < 0.01f)
    //    {
    //        activeForwardSpeed = MoveAxis.y * forwardSpeed;
    //        activeStrafeSpeed = MoveAxis.x * strafeSpeed;
    //    }
    //       
    //}

    void FixedUpdate()
    {
        if(FlyingModeState == true)
        {
           // Debug.Log("Player has entered flying state");
            Flying();
        }


    }

    private void Flying()
    {
        //Roll calculation - keyboard
        rb.AddRelativeTorque(Vector3.back * _input.roll1D * rollTorque * Time.deltaTime);
        //pitch - mouse
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-_input.pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        //Yaw - mouse
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(_input.pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        //Thrust
        // for controller
        if(_input.thrust1D > 0.1f || _input.thrust1D < -0.1f)
        {
            float currentThrust = thrust;

            rb.AddRelativeForce(Vector3.forward * _input.thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }
// ---------------------------------------------------------------------------
        if (_input.upDown1D > 0.1f || _input.upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * _input.upDown1D * upThrust * Time.deltaTime);
            verticalGlide = _input.upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.deltaTime);
            verticalGlide *= upDownGlideReduction;
        }
// ---------------------------------------------------------------------------
        if (_input.strafe1D > 0.1f || _input.strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * _input.strafe1D * upThrust * Time.deltaTime);
            verticalGlide = _input.strafe1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }


    }

    public void StartFlying()
    {
        //disable Other movement based scripts
        DashScript.enabled = false;
        TPC.enabled = false;
        CC.enabled = false;
        FlyingModeState = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerCharacter.SetActive(false);
        PlayerPlane.SetActive(true);

        if(FlyingActionMapEnabled == false)
        {
            _PInput.SwitchCurrentActionMap("Player Flying Mode");
            FlyingActionMapEnabled = true;
        }
    }

    public void DisableFlying()
    {
        //enable Other movement based scripts
        DashScript.enabled = true;
        TPC.enabled = true;
        CC.enabled = true;
        FlyingModeState = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerCharacter.SetActive(true);
        PlayerPlane.SetActive(false);

        if (FlyingActionMapEnabled == true)
        {
            _PInput.SwitchCurrentActionMap("Player");
            FlyingActionMapEnabled = false;
        }
            
    }

    //Flying Mode State Get & Set
    public void SetFlyingMode(bool Toggle)
    {
        FlyingModeState = Toggle;
    }

    public bool GetFlyingMode()
    {
        return FlyingModeState;
    }


}
