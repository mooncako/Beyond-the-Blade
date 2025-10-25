using MoreMountains.Tools;
using UnityEngine;

public struct NewAbilityCallbackEvent
{
    public string SkillId;
    public int Index;
    public bool IsSuccessfullyAdded;
    public Skill Skill;

    public NewAbilityCallbackEvent(string skillId, int index, bool isSuccessfullyAdded, Skill skill)
    {
        SkillId = skillId;
        Index = index;
        IsSuccessfullyAdded = isSuccessfullyAdded;
        Skill = skill;
    }

    public static NewAbilityCallbackEvent e;

    public static void Trigger(string skillId, int index, bool isSuccessfullyAdded, Skill skill)
    {
        e.SkillId = skillId;
        e.Index = index;
        e.IsSuccessfullyAdded = isSuccessfullyAdded;
        e.Skill = skill;
        MMEventManager.TriggerEvent(e);
    }
}
