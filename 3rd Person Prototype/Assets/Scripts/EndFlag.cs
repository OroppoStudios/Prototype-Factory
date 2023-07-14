using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFlag : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        SpeedRunTimer.Instance.TimerEnabled = false;
    }
}
