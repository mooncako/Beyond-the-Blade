using UnityEngine;

public class DamageInfo
{
    public float Amount { get; set; }
    public GameObject Victim { get; set; }
    public GameObject Source { get; set; }
    public GameObject Instigator { get; set; }
    public DamageType DamageType { get; set; }

    public DamageInfo(float amount, GameObject victim, GameObject source, GameObject instigator, DamageType damageType)
    {
        Amount = amount;
        Victim = victim;
        Source = source;
        Instigator = instigator;
        DamageType = damageType;
    }
}
