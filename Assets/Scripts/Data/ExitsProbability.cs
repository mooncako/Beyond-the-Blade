using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ExitsProbability
{
    [Range(0, 1), OnValueChanged("Normalize")] public float RegularExitPercentage = .6f;
    [Range(0, 1), OnValueChanged("Normalize")] public float ShopExitPercentage = .2f;
    [Range(0, 1), OnValueChanged("Normalize")] public float RecoveryExitPercentage = .2f;

    public void Normalize()
    {
        float sum = RegularExitPercentage + ShopExitPercentage + RecoveryExitPercentage;
        if (sum == 0) return;

        RegularExitPercentage /= sum;
        ShopExitPercentage /= sum;
        RecoveryExitPercentage /= sum;
    }
}
