using System;
using UnityEngine;

[Serializable]
public class PooledAbility
{
    public string SkillId;
    public bool CanAppear = true;

    public PooledAbility(PooledAbility copy)
    {
        SkillId = copy.SkillId;
        CanAppear = copy.CanAppear;
    }
}
