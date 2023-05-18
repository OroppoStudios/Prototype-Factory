using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRunTimer : MonoBehaviour
{
    float Timer = 0;
    bool TimerEnabled = false;

    private void Update()
    {
        if (TimerEnabled)
            Timer += Time.deltaTime;
    }
    public void SetTimer(float value)
    {
        Timer = value;
    }
    public float GetTimer()
    {
        return Timer;
    }
    public void StartTimer()
    {
        TimerEnabled = true;
    }
    public void StopTimer()
    {
        TimerEnabled = true;
    }


}
