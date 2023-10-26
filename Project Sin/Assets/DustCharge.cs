using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustCharge : MonoBehaviour
{
    [Range(0, 5)] public int DustCharges = 1, MaxDustCharges = 3;
    public static DustCharge Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    public bool ValidateCharge()
    {
        if (DustCharges > 0)
        {
            DustCharges--;
            return true;
        }
        else return false;
    }
    public bool ValidateCharge(int num)
    {
        if (DustCharges > 0)
        {
            DustCharges-= num;
            return true;
        }
        else return false;
    }
    public void UseCharge()
    {
        DustCharges--;
    }
    public void GainCharge()
    {
        DustCharges++;
        DustCharges = (DustCharges > MaxDustCharges) ? MaxDustCharges : DustCharges;
    }
    public void UseCharge(int num)
    {
        DustCharges -= num;
    }
    public void GainCharge(int num)
    {
        DustCharges += num;
        DustCharges = (DustCharges > MaxDustCharges) ? MaxDustCharges : DustCharges;
    }
}
