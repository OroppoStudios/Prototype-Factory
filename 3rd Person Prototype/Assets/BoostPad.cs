using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public Vector2 Direction;
    // Start is called before the first frame update

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + Vector3.up/2f,0.1f);
        Gizmos.DrawRay(transform.position + Vector3.up/2f, Quaternion.Euler(Direction)*Vector3.forward);
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CharacterMovement Character))
            return;

        Character.ActivateBoost(Quaternion.Euler(Direction) * Vector3.forward);
    }
}
