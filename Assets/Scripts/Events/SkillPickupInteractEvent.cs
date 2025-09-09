using MoreMountains.Tools;
using UnityEngine;

public struct SkillPickupInteractEvent
{
    public EventStateType Type;
    public PlayerController Controller;
    public Skill TargetSkill;

    public SkillPickupInteractEvent(EventStateType type, PlayerController controller, Skill targetSkill)
    {
        Type = type;
        Controller = controller;
        TargetSkill = targetSkill;
    }

    public static SkillPickupInteractEvent e;

    public static void Trigger(EventStateType type, PlayerController controller, Skill targetSkill)
    {
        e.Type = type;
        e.Controller = controller;
        e.TargetSkill = targetSkill;
        MMEventManager.TriggerEvent(e);
    }
}
