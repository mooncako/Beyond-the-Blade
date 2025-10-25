using MoreMountains.Tools;
using UnityEngine;

public struct SkillSwapEvent
{
    public Skill Skill;
    public string SkillId;

    public SkillSwapEvent(Skill skill, string skillId)
    {
        Skill = skill;
        SkillId = skillId;
    }

    public static SkillSwapEvent e;

    public static void Trigger(Skill skill, string skillId)
    {
        e.Skill = skill;
        e.SkillId = skillId;
        MMEventManager.TriggerEvent(e);
    }
}
