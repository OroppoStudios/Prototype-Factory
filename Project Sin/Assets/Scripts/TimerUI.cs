using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private SpeedRunTimer timerInstance;

    private TextMeshProUGUI text;

    private void Start()
    {
        timerInstance.StartTimer();

        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = timerInstance.GetTimer().ToString("0.000");
    }
}
