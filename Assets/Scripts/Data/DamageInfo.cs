using UnityEngine;

public class DamageInfo
{
    public float Amount { get; set; }
    public GameObject Victim { get; set; }
    public Health Health { get; set; }
    public GameObject Instigator { get; set; }
    public DamageType DamageType { get; set; }

    public DamageInfo(float amount, GameObject victim, Health health, GameObject instigator, DamageType damageType)
    {
        Amount = amount;
        Victim = victim;
        Health = health;
        Instigator = instigator;
        DamageType = damageType;
    }
}
