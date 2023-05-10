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
    
    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
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

        dashDuration = 0f; // Initialize the dash timer to zero
        //cooldownTimer = 0f; // Initialize the cooldown timer to zero

        _input = GetComponent<StarterAssetsInputs>();
        _PInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        //(Mouse.current.rightButton.wasPressedThisFrame == true);

        if (impact.magnitude > magnitudeCheck) cc.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 10 * Time.deltaTime);

        if (Mouse.current.rightButton.wasPressedThisFrame == true)
        {
            Debug.Log("Player should dash now");
            AddImpact(orientation.forward, dashForce);
        }
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }
}
