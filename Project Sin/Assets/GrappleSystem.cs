using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleSystem : MonoBehaviour
{
   
    public Camera ProjectionCamera, Camera;
    [Range(0, 50)] public float TetherRange = 25f;
    private int NumPoints = 0;
    protected readonly Vector2[] InitCornerPos = new Vector2[]
    {
        new Vector2(-160.0f,90.0f), new Vector2(-160.0f,-90.0f), new Vector2(160.0f,-90.0f),new Vector2(160.0f,90.0f)
    };
    protected const float InitFOV = 28.0f;
    public LayerMask WhatIsTetherable;
    public List<RectTransform> TargetingCorners;
    [Range(0, 1)] public float TargetingSystemUISize = 1, TetherZoomIntensity =0.25f;
    [Range(0, 3)] public float TetherHoldTime = 1, TetherReleaseTime, SPTetherTime=0.5f;
    [Range(50, 200)] public float DoublePointTetherSpeed = 100, SinglePointTetherSpeed = 100;
    private List<Vector3> TetherPoints = new List<Vector3>(2);
    LineRenderer lines;
    private bool TetherCharged = false;
    private float BaseCameraFOV;
    private void Awake()
    {
        PlayerInput.Tether += StartTether;
        TetherPoints.Add(Vector3.zero); 
        TetherPoints.Add(Vector3.zero);
        lines = gameObject.AddComponent<LineRenderer>();
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
    private void OnValidate()
    {
        BaseCameraFOV = Camera.fieldOfView;
        for (int i = 0; i <= 3; i++)
        {
            TargetingCorners[i].anchoredPosition = InitCornerPos[i] * TargetingSystemUISize;
        }
        ProjectionCamera.fieldOfView = InitFOV * TargetingSystemUISize;
    }
    public void StartTether()
    {

        Debug.Log("Tether");
        //TODO: 
        //figure out how many targets the chick is looking at 
        Collider[] colls = Physics.OverlapSphere(transform.position, TetherRange, WhatIsTetherable);
        var GrapplePoints = GetClosestEnemies(colls);

        NumPoints = GrapplePoints[0] != null ? 1 : 0;
        NumPoints = GrapplePoints[1] != null ? 2 : NumPoints;


        Debug.Log(NumPoints);


        switch (NumPoints)
        {
            case 0:
                GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.Standard);
                break;
            case 1:
                TetherPoints[0] = GrapplePoints[0].position;
                Invoke(nameof(StopSingleTetherMovement), SPTetherTime);
                GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.Tethering);
                break;
            case 2:
                TetherPoints[0] = GrapplePoints[0].position;
                TetherPoints[1] = GrapplePoints[1].position;
                StartCoroutine(HoldTether());
                GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.DoubleTethering);
                break;
        }

        //Debug.Log(TetherPoints[0].name);
        //Debug.Log(TetherPoints[1].name);
        // PlayersState.ChangeState(MoveState.Tethering);
    }
    public void SingleTether(Vector2 vec)
    {
        Debug.Log("single");
        lines.enabled = true;
        lines.positionCount = 2;
        lines.SetPosition(0, transform.position);
        lines.SetPosition(1, TetherPoints[0]);
        lines.SetWidth(0.05f, 0.05f);

        SingleTetherMovement();
    }
    public void DoubleTether(Vector2 vec)
    {
       
        Debug.Log("double");
        lines.enabled = true;
        lines.positionCount = 4;
        lines.SetPosition(0, transform.position);
        lines.SetPosition(1, TetherPoints[0]);
        lines.SetPosition(2, transform.position);
        lines.SetPosition(3, TetherPoints[1]);
        lines.SetWidth(0.05f, 0.05f);

        DoubleTetherMovement();
       // Destroy(lines);
    }
    #region TetherAbility

    
    private IEnumerator HoldTether()
    {
        float journey = 0f;
        while (journey <= TetherHoldTime)
        {
            Camera.fieldOfView = BaseCameraFOV-(BaseCameraFOV * (journey/TetherHoldTime)*(BaseCameraFOV*TetherZoomIntensity/BaseCameraFOV));
            yield return new WaitForSeconds(0.01f);
            journey += 0.01f;
        }     
        TetherCharged = true;
        StartCoroutine(ReleaseTether());
    }
    private IEnumerator ReleaseTether()
    {
        float journey = TetherReleaseTime;
        while (journey >= 0.0f)
        {
            Camera.fieldOfView = BaseCameraFOV - (BaseCameraFOV * (journey / TetherHoldTime) * (BaseCameraFOV * TetherZoomIntensity / BaseCameraFOV));
            yield return new WaitForSeconds(0.01f);
            journey -= 0.01f;
        }
        Camera.fieldOfView = BaseCameraFOV;
    }
    public void SingleTetherMovement()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().velocity = Camera.transform.rotation * Vector3.forward * SinglePointTetherSpeed/3.6f;   
    }
    public void DoubleTetherMovement()
    {
        if (!TetherCharged) return;

        Vector3 Midway = Vector3.Lerp(TetherPoints[0], TetherPoints[1], 0.5f);
        Vector3 Target = transform.position-(transform.position-Midway)*1.5f;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = (Target - transform.position).normalized * DoublePointTetherSpeed/3.6f;
       if((Target - transform.position).magnitude < 3.0f)
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().velocity = (Target - transform.position).normalized * DoublePointTetherSpeed / 3.6f;
            GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.Standard);
            StartCoroutine(GetComponent<CharacterMovement>().GeneralSlowDown(1, DoublePointTetherSpeed));
            lines.enabled = false;
            TetherCharged = false;
            //Invoke(nameof(test), 1.0f);
        }
           

        //lines.SetPosition(3, Target);
    }
    void StopSingleTetherMovement()
    {
        GetComponent<CharacterMovement>().PlayersState.ChangeState(MoveState.Standard);
        StartCoroutine(GetComponent<CharacterMovement>().GeneralSlowDown(1, SinglePointTetherSpeed));
        GetComponent<Rigidbody>().useGravity = true;
        lines.enabled = false;
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
                if ((dSqrToTarget < closestDistanceSqr) && !Targets.Contains(potentialTarget.transform) && IsVisible(ProjectionCamera, potentialTarget.gameObject))
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
