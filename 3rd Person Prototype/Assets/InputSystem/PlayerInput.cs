using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    Inputs InputActions;
    public static event Action Jump, Fire, GroundPound, Dash;
    public static event Action<Vector2> Move,Look;
    Vector2 move = Vector2.zero;
    // Start is called before the first frame update
    public void Awake()
    {
        InputActions = new Inputs();


       
    }
    // Update is called once per frame
    void LateUpdate()
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
        Debug.Log("skldf");
    }
    public void OnLook(InputValue action)
    {
       Look.Invoke(action.Get<Vector2>());
    }
    public void OnSprint(InputValue action)
    {

    }
    public void OnDash(InputValue action)
    {

    }
    public void OnGroundPound(InputValue action)
    {

    }
    void OnEnable()
    {
        InputActions.Player.Move.Enable();
    }
}
