using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRunTimer : MonoBehaviour
{
    public static SpeedRunTimer Instance;
    float Timer = 0;
   [HideInInspector] public bool TimerEnabled = false;

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

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
}
