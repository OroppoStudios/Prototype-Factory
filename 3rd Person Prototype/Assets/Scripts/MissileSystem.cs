using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
public class MissileSystem : MonoBehaviour
{
   
    [BoxGroup("Don't Touch These ")]public bool Tracking;
    [BoxGroup("Don't Touch These ")][HideInInspector] public  bool LockedOn;
    [BoxGroup("Don't Touch These ")]public Camera Camera, PlayerCamera;
    [BoxGroup("Don't Touch These ")]private Transform Target;
    [BoxGroup("Don't Touch These ")]public GameObject TargetIndicator;
    [BoxGroup("Don't Touch These ")]
    protected readonly Vector2[] InitCornerPos = new Vector2[]
    {
        new Vector2(-160.0f,90.0f), new Vector2(-160.0f,-90.0f), new Vector2(160.0f,-90.0f),new Vector2(160.0f,90.0f)
    };
    protected const float InitFOV = 28.0f;

    [BoxGroup("Don't Touch These ")]  public List<RectTransform> TargetingCorners;

    [BoxGroup("Touch These ")]
    [ShowAssetPreview(128, 128)]
    public GameObject MissilePrefab;

    [BoxGroup("Touch These ")] public LayerMask WhatIsTargetable;
    [Tooltip("UI size of targeting area")] [BoxGroup("Touch These ")][Range(0, 1)] public float TargetingSystemUISize = 1;
    [BoxGroup("Touch These ")][Range(0, 150)] public float MissileSeekRange = 50f;
    [BoxGroup("Touch These ")] [Range(0, 3)] public float LockOnTime = 1;
    [BoxGroup("Touch These ")] [Range(1, 300)] public float MissleSpeed = 25;
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

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Shoot();
    }
    
    private void Shoot()
    {
        GameObject Projectile = Instantiate(MissilePrefab,transform.position,Camera.transform.rotation);
        Projectile.transform.parent = null;
        Projectile.transform.Rotate(Vector3.up * -90);
        Projectile.GetComponent<Missle>().Target = Target;
        Projectile.GetComponent<Missle>().Tracking = LockedOn;
        Projectile.GetComponent<Missle>().Speed = MissleSpeed;
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
               TargetingSystemUISize* new Vector2((Camera.WorldToViewportPoint(T.position).x - 0.5f) * 320.0f,
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
