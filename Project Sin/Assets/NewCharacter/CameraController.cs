using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   
    [Range(0,20)] public float sensitivity =10.0f;
    [Range(45, 90)] public float lookXLimit = 45.0f;
    
    private Quaternion Direction;
    public Vector2 MouseInput  = Vector2.zero;

    private void Awake()
    {
        PlayerInput.Look += Look;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDestroy()
    {
        PlayerInput.Look -= Look;
    }
    void Update()
    {
        Direction = Quaternion.Euler(MouseInput.y, 0, 0);
        MouseInput.y = Mathf.Clamp(MouseInput.y, -lookXLimit, lookXLimit);
        
        //Set Rotations
        transform.parent.localRotation = Quaternion.Euler(0, MouseInput.x, 0);    
        gameObject.transform.localRotation = Direction;
    }
    public void Look(Vector2 Input)
    {
        MouseInput.x += Input.x * sensitivity;
        MouseInput.y += Input.y * sensitivity;
        // 
        //lo Input.GetAxis("Mouse Y")cks rota Input.GetAxis("Mouse Y")tion
        Direction = Quaternion.Euler(MouseInput.y, 0, 0);
        MouseInput.y = Mathf.Clamp(MouseInput.y, -lookXLimit, lookXLimit);

        //Set Rotations
        transform.parent.localRotation = Quaternion.Euler(0, MouseInput.x, 0);
        gameObject.transform.localRotation = Direction;
    }
}
