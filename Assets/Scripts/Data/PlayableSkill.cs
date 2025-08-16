using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayableSkill
{
    public float BaseWeight = 1f;
    public string SkillId;
    [ReadOnly] public bool IsInCooldown = false;
    
}
