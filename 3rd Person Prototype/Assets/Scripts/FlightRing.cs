using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I added this to prototype extending flight duration mid flight - Kai

public class FlightRing : MonoBehaviour
{
    private CharacterMovement CMObj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("FLIGHT TIME EXTENDED");
            CMObj = other.GetComponent<CharacterMovement>();
            CMObj.FlyExtended = true;
            
        }
    }
}
