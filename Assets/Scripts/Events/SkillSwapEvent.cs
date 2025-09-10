using MoreMountains.Tools;
using UnityEngine;

public struct SkillSwapEvent
{
    public Skill Skill;

    public SkillSwapEvent(Skill skill)
    {
        Skill = skill;
    }

    public static SkillSwapEvent e;

    public static void Trigger(Skill skill)
    {
        e.Skill = skill;
        MMEventManager.TriggerEvent(e);
    }
}
