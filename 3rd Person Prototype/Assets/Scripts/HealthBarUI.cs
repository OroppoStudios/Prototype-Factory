using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Image HealthBar;

    [SerializeField]
    public GameObject HealthComponentOverride;

    [SerializeField]
    private float updateSpeedSeconds = 0.5f;

    private void Awake()
    {

        if (HealthComponentOverride != null)
            HealthComponentOverride.GetComponent<HealthSystem>().OnHealthPercentChanged += HandleHealthChanged;
        else
            GetComponentInParent<HealthSystem>().OnHealthPercentChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {

        StartCoroutine(ChangeToPercent(pct));


        //HealthBar.fillAmount = pct;
    }

    private IEnumerator ChangeToPercent(float pct)
    {
        float preChangePercent = HealthBar.fillAmount;
        float elapsed = 0f;


        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            HealthBar.fillAmount = Mathf.Lerp(preChangePercent, pct, elapsed / updateSpeedSeconds);

            yield return null;
        }

        HealthBar.fillAmount = pct;
    }
}
