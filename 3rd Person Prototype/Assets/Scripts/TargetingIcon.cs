using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingIcon : MonoBehaviour
{
    public WaitForSeconds LockOnWait = new WaitForSeconds(1);
    [HideInInspector] public MissileSystem MissileSystem;
    void Update()
    {
        GetComponent<RectTransform>().RotateAround(transform.position, Vector3.forward, Time.deltaTime * 75);
    }
    private void OnEnable()
    {
        StartCoroutine(LockOn());
    }
    private void OnDisable()
    {
        Debug.Log("not Locked in");
        MissileSystem.LockedOn = false;
        StopAllCoroutines();
    }
    private IEnumerator LockOn()
    {
        yield return LockOnWait;
        Debug.Log("Locked in");
        MissileSystem.LockedOn = true;
    }
}
