using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
public struct ActiveMissle
{
    public ActiveMissle(Transform T, int num)
    {
        MissleTransform = T;
        TargetNum = num;
    }
    public Transform MissleTransform;
    public int TargetNum;
}
public class MissileSystem : MonoBehaviour
{
   
    [BoxGroup("Don't Touch These ")]public bool Tracking;
    [BoxGroup("Don't Touch These ")][HideInInspector] public  bool LockedOn;
    [BoxGroup("Don't Touch These ")]public Camera Camera, PlayerCamera;
    [BoxGroup("Don't Touch These ")]private Transform Target;
    [BoxGroup("Don't Touch These ")]public GameObject TargetIndicator;
    [BoxGroup("Don't Touch These ")] public List<GameObject> TargetIndicators;
    [BoxGroup("Don't Touch These ")]
    protected readonly Vector2[] InitCornerPos = new Vector2[]
    {
        new Vector2(-160.0f,90.0f), new Vector2(-160.0f,-90.0f), new Vector2(160.0f,-90.0f),new Vector2(160.0f,90.0f)
    };
    protected const float InitFOV = 28.0f;

    [BoxGroup("Don't Touch These ")]  public List<RectTransform> TargetingCorners;
    [BoxGroup("Don't Touch These ")] public List<Transform> Targets = new List<Transform>(5);
    [BoxGroup("Touch These ")]
    [ShowAssetPreview(64, 64)]
    public GameObject MissilePrefab;

    [BoxGroup("Touch These ")] public LayerMask WhatIsTargetable;
    [Tooltip("UI size of targeting area")] [BoxGroup("Touch These ")][Range(0, 1)] public float TargetingSystemUISize = 1;
    [BoxGroup("Touch These ")][Range(0, 150)] public float MissileSeekRange = 50f;
    [BoxGroup("Touch These ")] [Range(0, 3)] public float LockOnTime = 1;
    [BoxGroup("Touch These ")][Range(0, 5)] public int MaxTargets = 1;
    [BoxGroup("Touch These ")] [Range(1, 300)] public float MissleSpeed = 25;
    
    public List<ActiveMissle> ActiveMissles= new List<ActiveMissle>();
  
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
        Target = null;
       
        Collider[] colls = Physics.OverlapSphere(transform.position, MissileSeekRange, WhatIsTargetable);
       // Target = GetClosestEnemy(colls);
        
       //if (Target)
       //    TryTarget(Target);
       //else TargetIndicator.SetActive(false);
       //
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ShootMissles();

        List<Transform> Temps = new List<Transform>(Targets);
       
        Targets.Clear();
        Targets.TrimExcess();
        Targets = GetClosestEnemies(colls);

        ValidateTargetLockOn(Temps, Targets);
       


        if (Targets[0])
            ValidateIndicators(Targets);
        else for (int i = 0; i < MaxTargets; i++)
               TargetIndicators[i].SetActive((Targets[i]!=null) ? true : false);       
    }
    private void Awake()
    {
        foreach (GameObject Indicator in TargetIndicators)
        {
            Indicator.GetComponent<TargetingIcon>().LockOnWait = new WaitForSeconds(LockOnTime);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, MissileSeekRange);
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
    private void ShootMissles()
    {   
      GameObject Projectile = Instantiate(MissilePrefab,transform.position,Camera.transform.rotation);
      int BestTargetnum = ValidateBestTarget();
      ActiveMissle Missle = new ActiveMissle(Projectile.transform, BestTargetnum);
      ActiveMissles.Add(Missle);
      Projectile.GetComponent<Missle>().OnMissleHit += HandleMissleHit;

      
      Projectile.transform.parent = null;
      Projectile.transform.Rotate(Vector3.up * -90);
      Projectile.GetComponent<Missle>().Target = Targets[BestTargetnum];
      Projectile.GetComponent<Missle>().Tracking = TargetIndicators[BestTargetnum].GetComponent<TargetingIcon>().Locked;
      Projectile.GetComponent<Missle>().Speed = MissleSpeed;
       // // use this to shoot all targets that the system is currently locked on to 

    }
    private void HandleMissleHit(Transform T)
    {
     
        foreach (ActiveMissle Missle in ActiveMissles)
        {
          //  Debug.Log((T==Missle.MissleTransform) ? true : false);
            if (T == Missle.MissleTransform)
            {             
                ActiveMissles.Remove(Missle);
                ActiveMissles.TrimExcess();
                Destroy(T.gameObject);
                break;
            }       
        }
       // Debug.Log(T.name + " was Destroyed");
    }
    private void ValidateTargetLockOn(List<Transform> Current, List<Transform> Prevoius)
    {
        //check if the targets are the same as the ones in the previous frame
        int i = 0;
        foreach (Transform T in Current)
        {
            bool IsDifferentLockOn = ((Current[i] == Prevoius[i]) ? true : false);
            TargetIndicators[i].SetActive(IsDifferentLockOn);
            i++;
        }
    }

    private int ValidateBestTarget()
    {
        List<int> NumLockedOnMissles = new List<int>();
        for (int i = 0; i <MaxTargets; i++)
            NumLockedOnMissles.Add(0);
        
        foreach (ActiveMissle M in ActiveMissles)
        {
            Debug.Log(M.TargetNum +" TargetNum");
            Debug.Log(NumLockedOnMissles.Count+" TotalTargets");
            NumLockedOnMissles[M.TargetNum]++;
          
        }
        int LowestNUM = 99,Index=0;
        for (int i=0;i<NumLockedOnMissles.Count; i++)
        {
           // LowestNUM = (NumLockedOnMissles[i] < LowestNUM) ? NumLockedOnMissles[i] : LowestNUM;
            //Index = (NumLockedOnMissles[i] < LowestNUM) ? i : Index;
            if((NumLockedOnMissles[i] < LowestNUM))
            {
                LowestNUM = NumLockedOnMissles[i];
                Index = i;
            }
                Debug.Log(NumLockedOnMissles[i] + " Locked onto " + i);
        }
        Debug.Log("Index "+ Index+" Is Best with "+LowestNUM + " Locked Onto Target " );
        return Index; 
    }
    List<Transform>GetClosestEnemies(Collider[] enemies)
    {
        List<Transform> Targets = new List<Transform>();

        for (int i = 0; i < MaxTargets; i++)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Collider potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if ((dSqrToTarget < closestDistanceSqr) && !Targets.Contains(potentialTarget.transform)&& IsVisible(Camera, potentialTarget.gameObject))
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
            Targets.Add(bestTarget);
        }
        return Targets;
    }
    private void ValidateIndicators(List<Transform> Targets)
    {     
        for (int i = 0; i < Targets.Count; i++)
        {
            //  Debug.Log(Targets.Count);
            if (Targets[i] != null)
            {
                TargetIndicators[i].SetActive(true);
                TargetIndicators[i].GetComponent<RectTransform>().anchoredPosition =
                   TargetingSystemUISize * new Vector2((Camera.WorldToViewportPoint(Targets[i].position).x - 0.5f) * 320.0f,
                    (Camera.WorldToViewportPoint(Targets[i].position).y - 0.5f) * 180.0f);
            }
            else TargetIndicators[i].SetActive(false);
        }
    }
    Transform GetClosestEnemy(Collider[] enemies)
    {
        //legacy
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
                (Camera.WorldToViewportPoint(T.position).y - 0.5f) * 180.0f); 

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
