using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        MissileSystem.LockedOn = false;
        StopAllCoroutines();
    }
    private IEnumerator LockOn()
    {
        GetComponent<RawImage>().color = Color.red;
        yield return LockOnWait;
        MissileSystem.LockedOn = true;
        GetComponent<RawImage>().color = Color.green;
    }
}
