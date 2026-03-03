
using Sirenix.OdinInspector;
using UnityEngine;

public class Stats : ScriptableObject
{

    public StatsData StatsData;

    [ReadOnly] public float TempMaxHealth = 0;
    [ShowInInspector, ReadOnly] public float MaxHealth => StatsData.BaseMaxHealth + TempMaxHealth;

    [ReadOnly] public float TempMaxEnergy = 0f;
    [ShowInInspector, ReadOnly] public float MaxEnergy => StatsData.BaseMaxEnergy + TempMaxEnergy;

    [ReadOnly] public float TempMaxStamina = 0f;
    [ShowInInspector, ReadOnly] public float MaxStamina => StatsData.BaseMaxStamina + TempMaxStamina;

    [ReadOnly] public float TempStaminaRegeneration = 0f;
    [ShowInInspector, ReadOnly] public float StaminaRegeneration => StatsData.BaseStaminaRegeneration + TempStaminaRegeneration;

    [ReadOnly] public float TempDashStaminaCost = 0f;
    [ShowInInspector, ReadOnly] public float DashStaminaCost => StatsData.BaseDashStaminaCost + TempDashStaminaCost; // use minus? 

    [ReadOnly] public float TempParryStaminaCost = 0f;
    [ShowInInspector, ReadOnly] public float ParryStaminaCost => StatsData.BaseParryStaminaCost + TempParryStaminaCost; //  minus? 


    [ReadOnly] public float TempMovementSpeedMultiplier = 0;
    [ShowInInspector, ReadOnly] public float MovementSpeedMultiplier => StatsData.BaseMovementSpeedMultiplier + TempMovementSpeedMultiplier;


    [ReadOnly] public float TempDamageMultiplier = 0;
    [ShowInInspector, ReadOnly] public float DamageMultiplier => StatsData.BaseDamageMultiplier + TempDamageMultiplier;


    [ReadOnly] public float TempDamageReduction = 0f;
    [ShowInInspector, ReadOnly] public float DamageReduction => StatsData.BaseDamageReduction + TempDamageReduction;


    [ReadOnly] public float TempAttackSpeed = 0f;
    [ShowInInspector, ReadOnly] public float AttackSpeed => StatsData.BaseAttackSpeed + TempAttackSpeed;


    [ReadOnly] public float TempParryEnergyGain = 0f;
    [ShowInInspector, ReadOnly] public float ParryEnergyGain => StatsData.BaseParryEnergyGain + TempParryEnergyGain;


    [ReadOnly] public float TempDashEnergyGain = 0f;
    [ShowInInspector, ReadOnly] public float DashEnergyGain => StatsData.BaseDashEnergyGain + TempDashEnergyGain;


    [ReadOnly] public float TempHitStunDuration = 0f;
    [ShowInInspector, ReadOnly] public float HitStunDuration => StatsData.BaseHitStunDuration + TempHitStunDuration;


    [ReadOnly] public float TempRegulerStunDuration = 0f;
    [ShowInInspector, ReadOnly] public float RegulerStunDuration => StatsData.BaseRegulerStunDuration + TempRegulerStunDuration;


    [ReadOnly] public float TempResourceGainMultiplier = 0f;
    [ShowInInspector, ReadOnly] public float ResourceGainMultiplier => StatsData.BaseResourceGainMultiplier + TempResourceGainMultiplier;


    [ReadOnly] public float TempDashDistance = 0f;
    [ShowInInspector, ReadOnly] public float DashDistance => StatsData.BaseDashDistance + TempDashDistance;


    [ReadOnly] public float TempIframeDuration = 0f;
    [ShowInInspector, ReadOnly] public float IframeDuration => StatsData.BaseIframeDuration + TempIframeDuration;


    [Button]
    public virtual void Clear()
    {
        TempAttackSpeed = 0;
        TempDamageMultiplier = 0;
        TempDamageReduction = 0;
        TempMaxEnergy = 0;
        TempMaxHealth = 0;
        TempMovementSpeedMultiplier = 0;
        TempParryEnergyGain = 0;
        TempDashEnergyGain = 0;
        TempDashDistance = 0;
        TempHitStunDuration = 0;
        TempIframeDuration = 0;
        TempRegulerStunDuration = 0;
        TempResourceGainMultiplier = 0;
        TempStaminaRegeneration = 0;
    }

    public virtual void CopyValue(StatsData stats)
    {
        StatsData.Range = stats.Range;
        StatsData.AlertRange = stats.AlertRange;
        StatsData.FieldOfView = stats.FieldOfView;
        StatsData.BaseMaxHealth = stats.BaseMaxHealth;
        StatsData.BaseMaxEnergy = stats.BaseMaxEnergy;
        StatsData.BaseMaxStamina = stats.BaseMaxStamina;
        StatsData.BaseStaminaRegeneration = stats.BaseStaminaRegeneration;
        StatsData.BaseAttackSpeed = stats.BaseAttackSpeed;
        StatsData.BaseDamageMultiplier = stats.BaseDamageMultiplier;
        StatsData.BaseDamageReduction = stats.BaseDamageReduction;
        StatsData.BaseMovementSpeedMultiplier = stats.BaseMovementSpeedMultiplier;
        StatsData.BaseParryEnergyGain = stats.BaseParryEnergyGain;
        StatsData.BaseDashEnergyGain = stats.BaseDashEnergyGain;
        StatsData.BaseDashDistance = stats.BaseDashDistance;
        StatsData.BaseHitStunDuration = stats.BaseHitStunDuration;
        StatsData.BaseIframeDuration = stats.BaseIframeDuration;
        StatsData.BaseRegulerStunDuration = stats.BaseRegulerStunDuration;
        StatsData.BaseResourceGainMultiplier = stats.BaseRegulerStunDuration;
        Clear();
    }

}
