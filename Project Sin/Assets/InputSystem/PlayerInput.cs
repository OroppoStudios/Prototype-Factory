using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    Inputs InputActions;
    public static event Action Jump, Fire, GroundPound, Dash, FlyMode,Tether,Sprint;
    public static event Action<Vector2> Move,Look;
    Vector2 move = Vector2.zero;
    // Start is called before the first frame update
    public void Awake()
    {
        InputActions = new Inputs(); 
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Move.Invoke(move);
    }
    public void OnMove(InputValue action)
    {
        // Debug.Log(action.Get<Vector2>());
       move = (action.Get<Vector2>());
    }
    public void OnJump(InputValue action)
    {
        Jump.Invoke();
    }
    public void OnLook(InputValue action)
    {
       Look.Invoke(action.Get<Vector2>());
    }
 
    public void OnDash(InputValue action)
    {
        Dash.Invoke();
    }
    public void OnGroundPound(InputValue action)
    {
        GroundPound.Invoke();
    }
    public void OnFlyMode(InputValue action)
    {
        FlyMode.Invoke();
    }
    public void OnShoot(InputValue action)
    {
        Fire.Invoke();
    }
    public void OnTether(InputValue action)
    {
        Tether.Invoke();
    }
    public void OnSprint(InputValue action)
    {
        Sprint.Invoke();
    }
    void OnEnable()
    {
        InputActions.Player.Move.Enable();
    }
}
