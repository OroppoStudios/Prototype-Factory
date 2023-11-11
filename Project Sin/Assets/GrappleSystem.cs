using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleSystem : MonoBehaviour
{
    [Header("TetherDetails \n")]
    public Camera Camera;
    [Range(0, 50)] public float TetherRange = 25f;
    public LayerMask WhatIsTetherable;
    private void Awake()
    {
        PlayerInput.Tether += StartTether;
    }
    private void OnDestroy()
    {
        PlayerInput.Tether -= StartTether;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, TetherRange);
    }
    public void SingleTether(Vector2 vec)
    {

    }
    public void DoubleTether(Vector2 vec)
    {

    }
    #region TetherAbility

    public void StartTether()
    {
        GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.Tethering);
        Debug.Log("Tether");
        //TODO: 
        //figure out how many targets the chick is looking at 
        Collider[] colls = Physics.OverlapSphere(transform.position, TetherRange, WhatIsTetherable);
        var TetherPoints = GetClosestEnemies(colls);
        Debug.Log(TetherPoints[0].name);
        Debug.Log(TetherPoints[1].name);
        // PlayersState.ChangeState(MoveState.Tethering);
    }

    public void SingleTetherMovement(Vector2 vector)
    {

    }
    public void DoubleTetherMovement(Vector2 vector)
    {

    }
    List<Transform> GetClosestEnemies(Collider[] enemies)
    {
        List<Transform> Targets = new List<Transform>();

        for (int i = 0; i < 2; i++)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Collider potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if ((dSqrToTarget < closestDistanceSqr) && !Targets.Contains(potentialTarget.transform) && IsVisible(Camera, potentialTarget.gameObject))
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
            Targets.Add(bestTarget);
        }
        return Targets;
    }
    private bool IsVisible(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }
    #endregion TetherAbility
}
