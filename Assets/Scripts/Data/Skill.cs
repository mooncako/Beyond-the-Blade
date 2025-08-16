using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    public string AnimationID;
    public float Cooldown;
    public float Damage;
    public SkillAreaType AreaType;
    public SkillRarity Rarity;
    public bool TargetSelf;
    public List<string> Buffs = new List<string>();
    public List<string> Debuffs = new List<string>();
}
