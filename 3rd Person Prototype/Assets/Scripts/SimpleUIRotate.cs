using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUIRotate : MonoBehaviour
{
    
    void Update()
    {
        GetComponent<RectTransform>().RotateAround(transform.position, Vector3.forward, Time.deltaTime*75);
    }
}
