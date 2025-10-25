using MoreMountains.Tools;
using UnityEngine;

public struct SkillUpgradeEvent
{
    public EventStateType Type;
    public Weapon Weapon;
    public (string, UpgradeSlotType) Modifier;

    public SkillUpgradeEvent(EventStateType type, Weapon weapon, (string, UpgradeSlotType) modifier)
    {
        Type = type;
        Weapon = weapon;
        Modifier = modifier;
    }

    public static SkillUpgradeEvent e;

    public static void Trigger(EventStateType type, Weapon weapon, (string, UpgradeSlotType) modifier)
    {
        e.Type = type;
        e.Weapon = weapon;
        e.Modifier = modifier;
        MMEventManager.TriggerEvent(e);
    }
}
