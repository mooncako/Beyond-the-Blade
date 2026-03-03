using System;
using UnityEngine;

[Serializable]
public class StatsData
{
    [Header("Vision")]
    public float Range = 4f;
    public float AlertRange = 6f;
    public float FieldOfView = 140f;

    [Header("Stats")]
    public float BaseMaxHealth = 100f;

    public float BaseMaxEnergy = 10f;

    public float BaseMaxStamina = 10f;

    public float BaseStaminaRegeneration = 1f;

    public float BaseDashStaminaCost = 3f;

    public float BaseParryStaminaCost = 2f;

    public float BaseMovementSpeedMultiplier = 1;

    public float BaseDamageMultiplier = 1;

    public float BaseDamageReduction = 0;
    public float BaseAttackSpeed = 1f;

    public float BaseParryEnergyGain = 1.5f;

    public float BaseDashEnergyGain = .8f;

    public float BaseHitStunDuration = .5f;

    public float BaseRegulerStunDuration = 3f;

    public float BaseResourceGainMultiplier = 1f;

    public float BaseDashDistance = 5f;
    
    public float BaseIframeDuration = .2f;

}
