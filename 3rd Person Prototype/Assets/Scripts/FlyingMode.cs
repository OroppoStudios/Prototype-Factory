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

    private PlayerInput _PInput;
    private StarterAssetsInputs _input;


    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5.0f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;

    // Start is called before the first frame update
    void Start()
    {
        DashScript = GetComponent<NewDash>();
        TPC = GetComponent<ThirdPersonController>();
        CC = GetComponent<CharacterController>();

        _input = GetComponent<StarterAssetsInputs>();
        _PInput = GetComponent<PlayerInput>();

       // StartFlying();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 MoveAxis = context.ReadValue<Vector2>();

        if (MoveAxis.sqrMagnitude < 0.01f)
        {
            activeForwardSpeed = MoveAxis.y * forwardSpeed;
            activeStrafeSpeed = MoveAxis.x * strafeSpeed;
        }
           
    }

    public void StartFlying()
    {
        //disable Other movement based scripts
        DashScript.enabled = false;
        TPC.enabled = false;
        CC.enabled = false;
    }
}
