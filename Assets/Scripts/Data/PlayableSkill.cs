using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayableSkill
{
    public float BaseWeight = 1f;
    public string SkillId;
    [ReadOnly] public bool IsInCooldown = false;

    public PlayableSkill(string skillId, float baseWeight)
    {
        SkillId = skillId;
        BaseWeight = baseWeight;
    }

    public PlayableSkill(string skillId)
    {
        SkillId = skillId;
    }
}
