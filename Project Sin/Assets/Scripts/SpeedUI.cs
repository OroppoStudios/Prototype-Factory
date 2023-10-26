using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SpeedUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    public Rigidbody RB;
    public float SpeedLineThreshold = 75.0f;
    public GameObject Speedlines;
    // Start is called before the first frame update
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        Speedlines.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()

        //in case you need the extra 1.3f
    {   //+(1.3f* ((RB.velocity.magnitude > 0.1f) ? 1 : 0)))
        text.text = (RB.velocity.magnitude * 3.6f + (1.3f * ((RB.velocity.magnitude > 0.1f) ? 1 : 0))).ToString("0.0") + " KMPH";
        Speedlines.SetActive(RB.velocity.magnitude * 3.6f > SpeedLineThreshold);
    }
}
