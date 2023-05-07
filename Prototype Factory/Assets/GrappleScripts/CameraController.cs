using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   
    [Range(0,20)] public float sensitivity =10.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;
    private Quaternion Direction;
    public Vector2 MouseInput;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {

        MouseInput.x += Input.GetAxis("Mouse X") * sensitivity;
        MouseInput.y += Input.GetAxis("Mouse Y") * sensitivity;
        Direction = Quaternion.Euler(-MouseInput.y, 0, 0);
        transform.parent.localRotation = Quaternion.Euler(0, MouseInput.x, 0);    
        gameObject.transform.localRotation = Direction;
    }
}
