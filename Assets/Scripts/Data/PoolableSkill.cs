using System;
using UnityEngine;

[Serializable]
public class PoolableSkill
{
    public string SkillId;
    public bool CanAppear = true;

    public PoolableSkill(PoolableSkill skill)
    {
        SkillId = skill.SkillId;
        CanAppear = skill.CanAppear;
    }
}
