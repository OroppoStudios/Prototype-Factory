using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustChargePickup : MonoBehaviour
{

    [Range(0, 5)] public int ChargeStrength = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<DustCharge>().GainCharge(ChargeStrength);
            Destroy(gameObject);
        }
           
    }
}
