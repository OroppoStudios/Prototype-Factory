using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustCharge : MonoBehaviour
{
    [Range(0, 5)] public int DustCharges = 1, MaxDustCharges = 3;
    public static DustCharge Instance;
    // Start is called before the first frame update
    public List<Transform> BoostBars, BoostChargedBars;
    public Transform TopOfBar;
    private void OnValidate()
    {
        for (int i = 0; i < 5; i++)
            BoostBars[i].gameObject.SetActive(false);
        for (int i = 0; i < MaxDustCharges; i++)
            BoostBars[i].gameObject.SetActive(true);

        if(MaxDustCharges>0)
        TopOfBar.GetComponent<RectTransform>().localPosition = new Vector3(0f, BoostBars[MaxDustCharges-1].localPosition.y +20f, 0f);
        else TopOfBar.GetComponent<RectTransform>().localPosition = new Vector3(0f, -25f, 0f);

        AdjustChargeBar();

    }
    private void Awake()
    {
        if (!Instance)
            Instance = this;

        AdjustChargeBar();
    }
    public bool ValidateCharge()
    {
        if (DustCharges > 0)
        {
            UseCharge();
            return true;
        }
        else return false;
    }
    public bool ValidateCharge(int num)
    {
        if (DustCharges > 0)
        {
            UseCharge(num);
            return true;
        }
        else return false;
    }


    private void AdjustChargeBar()
    {
        DustCharges = (MaxDustCharges < DustCharges) ? MaxDustCharges : DustCharges;

        for (int i = 0; i < 5; i++)
            BoostChargedBars[i].gameObject.SetActive(false);
        for (int i = 0; i < DustCharges; i++)
            BoostChargedBars[i].gameObject.SetActive(true);
    }


    private void UseCharge()
    {
        DustCharges--;
        AdjustChargeBar();
    }
    public void GainCharge()
    {
        DustCharges++;
        DustCharges = (DustCharges > MaxDustCharges) ? MaxDustCharges : DustCharges;
        AdjustChargeBar();
    }
    private void UseCharge(int num)
    {
        DustCharges -= num;
        AdjustChargeBar();
    }
    public void GainCharge(int num)
    {
        DustCharges += num;
        DustCharges = (DustCharges > MaxDustCharges) ? MaxDustCharges : DustCharges;
         AdjustChargeBar();
    }
}
