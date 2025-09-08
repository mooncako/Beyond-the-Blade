
using Sirenix.OdinInspector;
using UnityEngine;

public class Stats : ScriptableObject
{
    [Header("Vision")]
    public float Range = 4f;
    public float AlertRange = 6f;
    public float FieldOfView = 140f;

    [Header("Stats")]
    public float BaseMaxHealth = 100f;
    [ReadOnly] public float TempMaxHealth = 0;
    [ShowInInspector, ReadOnly] public float MaxHealth => BaseMaxHealth + TempMaxHealth;

    public float BaseMaxEnergy = 10f;
    [ReadOnly] public float TempMaxEnergy = 0f;
    [ShowInInspector, ReadOnly] public float MaxEnergy => BaseMaxEnergy + TempMaxEnergy;

    public float BaseMovementSpeedMultiplier = 1;
    [ReadOnly] public float TempMovementSpeedMultiplier = 0;
    [ShowInInspector, ReadOnly] public float MovementSpeedMultiplier => BaseMovementSpeedMultiplier + TempMovementSpeedMultiplier;

    public float BaseDamageMultiplier = 1;
    [ReadOnly] public float TempDamageMultiplier = 0;
    [ShowInInspector, ReadOnly] public float DamageMultiplier => BaseDamageMultiplier + TempDamageMultiplier;

    public float BaseDamageReduction = 0;
    [ReadOnly] public float TempDamageReduction = 0f;
    [ShowInInspector, ReadOnly] public float DamageReduction => BaseDamageReduction + TempDamageReduction;

    public float BaseAttackSpeed = 1f;
    [ReadOnly] public float TempAttackSpeed = 0f;
    [ShowInInspector, ReadOnly] public float AttackSpeed => BaseAttackSpeed + TempAttackSpeed;

    public float BaseParryEnergyGain = 1.5f;
    [ReadOnly] public float TempParryEnergyGain = 0f;
    [ShowInInspector, ReadOnly] public float ParryEnergyGain => BaseParryEnergyGain + TempParryEnergyGain;

    public float BaseDashEnergyGain = .8f;
    [ReadOnly] public float TempDashEnergyGain = 0f;
    [ShowInInspector, ReadOnly] public float DashEnergyGain => BaseDashEnergyGain + TempDashEnergyGain;

    public float BaseHitStunDuration = .5f;
    [ReadOnly] public float TempHitStunDuration = 0f;
    [ShowInInspector, ReadOnly] public float HitStunDuration => BaseHitStunDuration + TempHitStunDuration;

    public float BaseRegulerStunDuration = 3f;
    [ReadOnly] public float TempRegulerStunDuration = 0f;
    [ShowInInspector, ReadOnly] public float RegulerStunDuration => BaseRegulerStunDuration + TempRegulerStunDuration;

    public float BaseResourceGainMultiplier = 1f;
    [ReadOnly] public float TempResourceGainMultiplier = 0f;
    [ShowInInspector, ReadOnly] public float ResourceGainMultiplier => BaseResourceGainMultiplier + TempResourceGainMultiplier;

    public float BaseDashForce = 1000f;
    [ReadOnly] public float TempDashForce = 0f;
    [ShowInInspector, ReadOnly] public float DashForce => BaseDashForce + TempDashForce;

    public float BaseIframeDuration = .2f;
    [ReadOnly] public float TempIframeDuration = 0f;
    [ShowInInspector, ReadOnly] public float IframeDuration => BaseIframeDuration + TempIframeDuration;


    public void Clear()
    {
        TempAttackSpeed = 0;
        TempDamageMultiplier = 0;
        TempDamageReduction = 0;
        TempMaxEnergy = 0;
        TempMaxHealth = 0;
        TempMovementSpeedMultiplier = 0;
        TempParryEnergyGain = 0;
        TempDashEnergyGain = 0;
        TempDashForce = 0;
        TempHitStunDuration = 0;
        TempIframeDuration = 0;
        TempRegulerStunDuration = 0;
        TempResourceGainMultiplier = 0;
    }
}
