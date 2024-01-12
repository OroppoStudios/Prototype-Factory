using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I added this to prototype extending flight duration mid flight - Kai

public class FlightRing : MonoBehaviour
{
    [HideInInspector] public bool ChargeUsed=false;
    public Vector2 Direction;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + Vector3.up / 2f, 0.1f);
        Gizmos.DrawRay(transform.position + Vector3.up / 2f, Quaternion.Euler(Direction) * Vector3.forward);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (ChargeUsed)
            return;

        if (!other.TryGetComponent(out CharacterMovement Character))
            return;

        Character.ActivateCharge(Quaternion.Euler(Direction) * Vector3.forward);
        ChargeUsed = true;
    }
}
