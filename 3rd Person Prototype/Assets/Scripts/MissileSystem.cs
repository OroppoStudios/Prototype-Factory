using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MissileSystem : MonoBehaviour
{
    [Header("Don't Touch These \n")]
    public bool Tracking;
    [HideInInspector] public bool LockedOn;
    public Camera Camera, PlayerCamera;
    private Transform Target;
    public GameObject TargetIndicator;
    protected readonly Vector2[] InitCornerPos = new Vector2[]
    {
        new Vector2(-160.0f,90.0f), new Vector2(-160.0f,-90.0f), new Vector2(160.0f,-90.0f),new Vector2(160.0f,90.0f)
    };
    protected const float InitFOV = 28.0f;
   
    public List<RectTransform> TargetingCorners;

    [Header("Touch These \n")]
    public LayerMask WhatIsTargetable;
    [Range(0, 1)] public float TargetingSystemUISize = 1;
    [Range(0, 150)] public float MissileSeekRange = 50f;
    [Range(0, 3)] public float LockOnTime = 1;
    private void OnValidate()
    {
        for (int i =0; i <= 3; i++)
        {
            TargetingCorners[i].anchoredPosition = InitCornerPos[i] * TargetingSystemUISize;
        }
        Camera.fieldOfView = InitFOV * TargetingSystemUISize;
    }
    // Update is called once per frame
    void Update()
    {
        Collider[] Targets = Physics.OverlapSphere(transform.position, MissileSeekRange, WhatIsTargetable);
        Target = GetClosestEnemy(Targets);
        if(Target)
            TryTarget(Target);

      
    }
    private void Awake()
    {
        TargetIndicator.GetComponent<TargetingIcon>().LockOnWait = new WaitForSeconds(LockOnTime);
        TargetIndicator.GetComponent<TargetingIcon>().MissileSystem = this;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, MissileSeekRange);
    }
    Transform GetClosestEnemy(Collider[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }
    void TryTarget(Transform T)
    {
        Tracking = (IsVisible(Camera, T.gameObject));

        TargetIndicator.SetActive(Tracking);

        TargetIndicator.GetComponent<RectTransform>().anchoredPosition =
                new Vector2((Camera.WorldToViewportPoint(T.position).x - 0.5f) * 320.0f,
                (Camera.WorldToViewportPoint(T.position).y - 0.5f) * 180.0f); ;

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
}
