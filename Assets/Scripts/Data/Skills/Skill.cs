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
    public WeaponType WeaponType;
    public SkillRange SkillRange;
    public Rarity Rarity;
    public bool TargetSelf;
    public bool IsTargetedGroundAOE;
    public List<(string, float)> Buffs = new List<(string, float)>();
    public List<(string, float)> Debuffs = new List<(string, float)>();
    public string Name;
    public string Description;
    public bool IsStartBuffed = false;
    public bool IsMidBuffed = false;
    public bool IsEndBuffed = false;
    [PreviewField] public Sprite Icon;
    public VFXInfo VFXInfo = new VFXInfo(Vector3.zero, Quaternion.identity, Vector3.one, false, false);

    public Skill(Skill skill)
    {
        AnimationID = skill.AnimationID;
        Cooldown = skill.Cooldown;
        Damage = skill.Damage;
        WeaponType = skill.WeaponType;
        SkillRange = skill.SkillRange;
        Rarity = skill.Rarity;
        TargetSelf = skill.TargetSelf;
        IsTargetedGroundAOE = skill.IsTargetedGroundAOE;
        for (int i = 0; i < skill.Buffs.Count; i++)
        {
            Buffs.Add(skill.Buffs[i]);
        }

        for (int i = 0; i < skill.Debuffs.Count; i++)
        {
            Debuffs.Add(skill.Debuffs[i]);
        }

        Name = skill.Name;
        Description = skill.Description;
        IsStartBuffed = skill.IsStartBuffed;
        IsMidBuffed = skill.IsMidBuffed;
        IsEndBuffed = skill.IsEndBuffed;
        Icon = skill.Icon;
        VFXInfo = skill.VFXInfo;
    }

    public Skill()
    {

    }
}
