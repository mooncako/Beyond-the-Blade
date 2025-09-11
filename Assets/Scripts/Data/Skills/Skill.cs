using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Skill
{
    public string AnimationID;
    public float Cooldown;
    public float Damage;
    public SkillRange SkillRange;
    public Rarity Rarity;
    public bool TargetSelf;
    public bool IsTargetedGroundAOE;
    public List<(string, float)> Buffs = new List<(string, float)>();
    public List<(string, float)> Debuffs = new List<(string, float)>();
    [ReadOnly] public bool IsStartBuffed = false;
    [ReadOnly] public bool IsMidBuffed = false;
    [ReadOnly] public bool IsEndBuffed = false;
}
