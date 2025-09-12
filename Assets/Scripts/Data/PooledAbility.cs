using System;
using UnityEngine;

[Serializable]
public class PooledAbility
{
    public string SkillId;
    public bool CanAppear = true;
    public float PossibilityIndex;

    public PooledAbility(PooledAbility copy)
    {
        SkillId = copy.SkillId;
        CanAppear = copy.CanAppear;
        PossibilityIndex = copy.PossibilityIndex;
    }
}
