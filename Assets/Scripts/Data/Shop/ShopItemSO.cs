using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("Item Info")]
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public int Cost;

    [Header("Item Type & Effects")]
    public ItemType ItemType = ItemType.Consumable;
    public ItemEffectType EffectType = ItemEffectType.RestoreHealth;
    public UpgradeType UpgradeType = UpgradeType.InGame;
    
    [Header("Effect Values")]
    [ShowIf("@EffectType == ItemEffectType.RestoreHealth")]
    public float HealthRestoreAmount = 25f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseMaxHealth")]
    public float MaxHealthIncrease = 10f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseDamage")]
    public float DamageIncrease = 0.1f; // 10% increase
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseAttackSpeed")]
    public float AttackSpeedIncrease = 0.1f; // 10% increase
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseDamageReduction")]
    public float DamageReductionIncrease = 0.05f; // 5% increase
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseMovementSpeed")]
    public float MovementSpeedIncrease = 0.1f; // 10% increase
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseDashForce")]
    public float DashForceIncrease = 100f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseMaxEnergy")]
    public float MaxEnergyIncrease = 2f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseParryEnergyGain")]
    public float ParryEnergyGainIncrease = 0.2f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseDashEnergyGain")]
    public float DashEnergyGainIncrease = 0.1f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseResourceGainMultiplier")]
    public float ResourceGainMultiplierIncrease = 0.1f; // 10% increase
    
    [ShowIf("@EffectType == ItemEffectType.DecreaseHitStunDuration")]
    public float HitStunDurationDecrease = 0.1f;
    
    [ShowIf("@EffectType == ItemEffectType.IncreaseIframeDuration")]
    public float IframeDurationIncrease = 0.05f;

    [ShowIf("@EffectType == ItemEffectType.IncreaseMaxStamina")]
    public float MaxStaminaIncrease = 2f;

    [ShowIf("@EffectType == ItemEffectType.IncreaseStaminaRegenrate")]
    public float StaminaRegenerationIncrease = 0.2f;



    [Header("Settings")]
    public bool CanPurchaseMultiple = false;
    public int MaxPurchaseCount = 1;

    public virtual void ApplyEffect()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player==null)
        {
            Debug.LogError($"Could not find PlayerController to apply effect of {ItemName}");
            return;
        }

        ApplyEffectToPlayer(player);
    }

    private void ApplyEffectToPlayer(PlayerController player)
    {
        switch (EffectType)
        {
            case ItemEffectType.RestoreHealth:
                RestorePlayerHealth(player, HealthRestoreAmount);
                break;
                
            case ItemEffectType.IncreaseMaxHealth:
                IncreasePlayerMaxHealth(player, MaxHealthIncrease, UpgradeType);
                break;
                
            case ItemEffectType.IncreaseDamage:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempDamageMultiplier += DamageIncrease;
                else
                    player.Stats.StatsData.BaseDamageMultiplier += DamageIncrease;
                break;
                
            case ItemEffectType.IncreaseAttackSpeed:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempAttackSpeed += AttackSpeedIncrease;
                else
                    player.Stats.StatsData.BaseAttackSpeed += AttackSpeedIncrease;
                break;
                
            case ItemEffectType.IncreaseDamageReduction:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempDamageReduction += DamageReductionIncrease;
                else
                    player.Stats.StatsData.BaseDamageReduction += DamageReductionIncrease;
                break;
                
            case ItemEffectType.IncreaseMovementSpeed:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempMovementSpeedMultiplier += MovementSpeedIncrease;
                else
                    player.Stats.StatsData.BaseMovementSpeedMultiplier += MovementSpeedIncrease;
                break;
                
            case ItemEffectType.IncreaseDashForce:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempDashDistance += DashForceIncrease;
                else
                    player.Stats.StatsData.BaseDashDistance += DashForceIncrease;
                break;
                
            case ItemEffectType.IncreaseMaxEnergy:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempMaxEnergy += MaxEnergyIncrease;
                else
                    player.Stats.StatsData.BaseMaxEnergy += MaxEnergyIncrease;
                player.Energy.ApplyStats(player.Stats); // Update energy component
                break;
                
            case ItemEffectType.IncreaseParryEnergyGain:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempParryEnergyGain += ParryEnergyGainIncrease;
                else
                    player.Stats.StatsData.BaseParryEnergyGain += ParryEnergyGainIncrease;
                break;
                
            case ItemEffectType.IncreaseDashEnergyGain:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempDashEnergyGain += DashEnergyGainIncrease;
                else
                    player.Stats.StatsData.BaseDashEnergyGain += DashEnergyGainIncrease;
                break;
                
            case ItemEffectType.IncreaseResourceGainMultiplier:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempResourceGainMultiplier += ResourceGainMultiplierIncrease;
                else
                    player.Stats.StatsData.BaseResourceGainMultiplier += ResourceGainMultiplierIncrease;
                break;
                
            case ItemEffectType.DecreaseHitStunDuration:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempHitStunDuration -= HitStunDurationDecrease;
                else
                    player.Stats.StatsData.BaseHitStunDuration -= HitStunDurationDecrease;
                break;
                
            case ItemEffectType.IncreaseIframeDuration:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempIframeDuration += IframeDurationIncrease;
                else
                    player.Stats.StatsData.BaseIframeDuration += IframeDurationIncrease;
                break;
            case ItemEffectType.IncreaseMaxStamina:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempMaxStamina += MaxEnergyIncrease;
                else
                    player.Stats.StatsData.BaseMaxStamina += MaxEnergyIncrease;
                player.Stamina.ApplyStats(player.Stats); 
                break;
            case ItemEffectType.IncreaseStaminaRegenrate:
                if(UpgradeType == UpgradeType.InGame)
                    player.Stats.TempStaminaRegeneration += MaxEnergyIncrease;
                else
                    player.Stats.StatsData.BaseStaminaRegeneration += MaxEnergyIncrease;
                player.Stamina.ApplyStats(player.Stats); 
                break;
            default:
                Debug.LogWarning($"Effect type {EffectType} not implemented for item {ItemName}");
                break;
        }

        Debug.Log($"Applied {EffectType} effect from {ItemName} to player");
    }

    private void RestorePlayerHealth(PlayerController player, float amount)
    {
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
            Debug.Log($"Restored {amount} health to player");
        }
        else
        {
            Debug.LogError("Could not find Health component on player");
        }
    }

    private void IncreasePlayerMaxHealth(PlayerController player, float amount, UpgradeType upgradeType)
    {
        if (upgradeType == UpgradeType.InGame)
            player.Stats.TempMaxHealth += amount;
        else
            player.Stats.StatsData.BaseMaxHealth += amount;
        
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.UpdateMaxHealth(player.Stats.MaxHealth);
            // Also restore some health when max health increases
            Debug.Log($"Increased player max health by {amount}");
        }
    }
}
