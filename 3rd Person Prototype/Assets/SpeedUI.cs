using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SpeedUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    public Rigidbody RB;
    // Start is called before the first frame update
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = (RB.velocity.magnitude * 3.6f).ToString("0.0") + " KMPH";
    
    }
}
