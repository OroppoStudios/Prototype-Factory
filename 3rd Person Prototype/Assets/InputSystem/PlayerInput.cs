using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    PlayerInputs InputActions;
    public event Action Jump, Fire, GroundPound, Dash;
    Vector2 move;
    // Start is called before the first frame update
    private void Awake()
    {
        InputActions = new PlayerInputs();
        //  InputActions.Player.Move.performed += cntxt => move = Vector2.zero;
        InputActions.Player.Move.performed += cntxt => move = cntxt.ReadValue<Vector2>();
        //InputActions.Player.Jump.performed += JumpAction;
    }
    // Update is called once per frame
    void Update()
    {
      //  Debug.Log(move);
    }
    public void OnMove(InputValue action)
    {
        Debug.Log(action.Get<Vector2>());
    }
    void JumpAction()
    {

    }
    void OnEnable()
    {
      //  fireAction.Enable();
    }
}
