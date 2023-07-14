using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    public bool Tracking;
    public Camera Camera;
    public Transform Target;
    protected readonly Vector2[] InitCornerPos = new Vector2[]
    {
        new Vector2(-160.0f,90.0f), new Vector2(-160.0f,-90.0f), new Vector2(160.0f,-90.0f),new Vector2(160.0f,90.0f)
    };
   protected const float InitFOV = 28.0f;
    [Range(0,1)] public float TargetingSystemSize=1;
    public List<RectTransform> TargetingCorners;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnValidate()
    {
        for (int i =0; i <= 3; i++)
        {
            TargetingCorners[i].anchoredPosition = InitCornerPos[i] * TargetingSystemSize;
        }
        Camera.fieldOfView = InitFOV * TargetingSystemSize;
    }
    // Update is called once per frame
    void Update()
    {
     
       // Debug.Log(Camera.WorldToViewportPoint(Target.position));
        Debug.Log(IsVisible(Camera, Target.gameObject));
       //Ray ray = Camera.ScreenPointToRay(Target.position);
       //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
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
