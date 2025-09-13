using System;

[Serializable]
public enum ItemType
{
    Consumable,
    Upgrade,
    Utility
}

[Serializable]
public enum ItemEffectType
{
    // Health Effects
    RestoreHealth,
    IncreaseMaxHealth,
    
    // Combat Stats
    IncreaseDamage,
    IncreaseAttackSpeed,
    IncreaseDamageReduction,
    
    // Movement Stats
    IncreaseMovementSpeed,
    IncreaseDashForce,
    
    // Energy Stats
    IncreaseMaxEnergy,
    IncreaseParryEnergyGain,
    IncreaseDashEnergyGain,
    
    // Special Stats
    IncreaseResourceGainMultiplier,
    DecreaseHitStunDuration,
    IncreaseIframeDuration
}
