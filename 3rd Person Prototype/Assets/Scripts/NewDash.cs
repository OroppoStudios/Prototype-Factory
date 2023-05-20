using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class NewDash : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private ThirdPersonController TPC;
    private CharacterController cc;
    private FlyingMode FlyMode;
    
    [Header("Dashing")]
    public float dashForce;
    //public float dashUpwardForce;
    public float dashDuration;
    public float DashCooldown;
    public float BoostWindow;
    public float FlyWindow;

    private float _verticalVelocity;
    private Vector3 _verticalVec3;
    private float _dashCooldownDelta;
    private float _boostWindowDelta;
    private float _flyWindowDelta;
    private bool canDash = true;
    private bool windowOpen = false;
    private bool flyWindowOpen = false;
    private bool isDashing = false;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Tooltip("The height the player can jump")]
    public float DashHeight = 1.2f;

    //Start is called before the first frame update

    private PlayerInput _PInput;
    private StarterAssetsInputs _input;

    float mass = 3.0f; // defines the character mass
    public float magnitudeCheck = 0.2f;
    Vector3 impact = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the rigid body component
        cc = GetComponent<CharacterController>(); // Get the character controller component
        TPC = GetComponent<ThirdPersonController>();
        FlyMode = GetComponent<FlyingMode>();

        dashDuration = 0f; // Initialize the dash timer to zero
        //cooldownTimer = 0f; // Initialize the cooldown timer to zero

        _input = GetComponent<StarterAssetsInputs>();
        _PInput = GetComponent<PlayerInput>();

        _verticalVec3.x = 0.0f;
        _verticalVec3.y = 0.0f;
        _verticalVec3.z = 0.0f;
    }

    void Update()
    {
        //Cooldown Timer
        if (_dashCooldownDelta >= 0.0f && canDash == false)
        {
            _dashCooldownDelta -= Time.deltaTime;
        }
        else if (_dashCooldownDelta <= 0.0f)
        {
            _dashCooldownDelta = DashCooldown;
            canDash = true;
        }

        BoostWindowTimer();
        FlyWindowTimer();

        if (impact.magnitude > magnitudeCheck)
        {
            cc.Move((impact * Time.deltaTime) + _verticalVec3 * Time.deltaTime);
            //cc.Move((impact * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            //cc.Move(impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 10 * Time.deltaTime);
        _verticalVec3 = Vector3.Lerp(_verticalVec3, Vector3.zero, 10 * Time.deltaTime);

        if (Mouse.current.rightButton.wasPressedThisFrame == true && canDash == true)
        {
            //Debug.Log("Player should dash now");
            AddImpact(orientation.forward, dashForce);
            canDash = false;
        }
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BasicEnemy")
        {
            Debug.Log("Enemy has entered the trigger");
            other.gameObject.SetActive(false);
            windowOpen = true;
        }
    }

    private void BoostWindowTimer()
    {
        //Input Window Timer
        if (_boostWindowDelta >= 0.0f && windowOpen == true)
        {
            _boostWindowDelta -= Time.deltaTime;

            //Still need to stop player from pressing ctrl multiple times during boost window ************************************************
            //can probably be fixed by a bool in the if statement
            if(Keyboard.current.ctrlKey.wasPressedThisFrame == true)
            {
                Debug.Log("Key was pressed during boost window");
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(DashHeight * -2f * Gravity);
                _verticalVec3.y = _verticalVelocity;
                AddImpact(orientation.forward, dashForce);
                flyWindowOpen = true;
            }
        }
        else if (_boostWindowDelta <= 0.0f)
        {
            _boostWindowDelta = BoostWindow;
            windowOpen = false;
        }
    }


    private void FlyWindowTimer()
    {
        //Input Window Timer
        if (_flyWindowDelta >= 0.0f && flyWindowOpen == true)
        {
            _flyWindowDelta -= Time.deltaTime;

            //Still need to stop player from pressing ctrl multiple times during boost window ************************************************
            //can probably be fixed by a bool in the if statement
            if (Keyboard.current.eKey.wasPressedThisFrame == true)
            {
                Debug.Log("Key was pressed during Fly window");
                FlyMode.StartFlying();
            }
        }
        else if (_flyWindowDelta <= 0.0f)
        {
            _flyWindowDelta = FlyWindow;
            flyWindowOpen = false;
        }
    }
}
