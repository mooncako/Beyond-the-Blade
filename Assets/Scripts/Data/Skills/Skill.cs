using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    public string AnimationID;
    public float Cooldown;
    public float Damage;
    public SkillRange SkillRange;
    public SkillRarity Rarity;
    public bool TargetSelf;
    public bool IsTargetedGroundAOE;
    public List<(string,float)> Buffs = new List<(string,float)>();
    public List<(string,float)> Debuffs = new List<(string,float)>();
}
