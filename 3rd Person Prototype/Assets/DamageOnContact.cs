using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageOnContact : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
            HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
            health.ModifyHealth(-1);
            Destroy(gameObject);
        }
    }
}
