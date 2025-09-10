using MoreMountains.Tools;
using UnityEngine;

public struct SkillPickupInteractEvent
{
    public EventStateType Type;
    public PlayerController Controller;
    public Skill TargetSkill;
    public string SkillId;

    public SkillPickupInteractEvent(EventStateType type, PlayerController controller, Skill targetSkill, string skillId)
    {
        Type = type;
        Controller = controller;
        TargetSkill = targetSkill;
        SkillId = skillId;
    }

    public static SkillPickupInteractEvent e;

    public static void Trigger(EventStateType type, PlayerController controller, Skill targetSkill, string skillId)
    {
        e.Type = type;
        e.Controller = controller;
        e.TargetSkill = targetSkill;
        e.SkillId = skillId;
        MMEventManager.TriggerEvent(e);
    }
}
