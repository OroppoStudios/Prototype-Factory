using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void OnBecameVisible()
    {
        Debug.Log("visable");
    }
    private void OnBecameInvisible()
    {
        Debug.Log("invisable");
    }
}
