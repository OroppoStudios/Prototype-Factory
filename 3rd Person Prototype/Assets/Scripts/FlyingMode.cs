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

    public bool FlyingModeState = false;

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

    private void Update()
    {
        if(FlyingModeState == true)
        {
            Debug.Log("Player has entered flying state");
        }
    }



    public void StartFlying()
    {
        //disable Other movement based scripts
        DashScript.enabled = false;
        TPC.enabled = false;
        CC.enabled = false;
        FlyingModeState = true;
    }

    public void DisableFlying()
    {
        //enable Other movement based scripts
        DashScript.enabled = true;
        TPC.enabled = true;
        CC.enabled = true;
        FlyingModeState = false;
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
