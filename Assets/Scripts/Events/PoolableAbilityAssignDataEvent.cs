using System;
using UnityEngine;

public struct PoolableAbilityAssignDataEvent
{
    public string AbilityOne;
    public string AbilityTwo;
    public string AbilityThree;

    public PoolableAbilityAssignDataEvent(string abilityOne, string abilityTwo, string abilityThree)
    {
        AbilityOne = abilityOne;
        AbilityTwo = abilityTwo;
        AbilityThree = abilityThree;
    }

    public static PoolableAbilityAssignDataEvent e;

    public static void Trigger(string abilityOne, string abilityTwo, string abilityThree)
    {
        e.AbilityOne = abilityOne;
        e.AbilityTwo = abilityTwo;
        e.AbilityThree = abilityThree;
    }
}
