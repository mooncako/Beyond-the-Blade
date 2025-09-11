using MoreMountains.Tools;
using UnityEngine;

public struct SkillUpgradePickupInteractEvent
{
    public EventStateType Type;
    public Weapon Weapon;
    public (string, UpgradeSlotType) Modifier;

    public SkillUpgradePickupInteractEvent(EventStateType type, Weapon weapon, (string, UpgradeSlotType) modifier)
    {
        Type = type;
        Weapon = weapon;
        Modifier = modifier;
    }

    public static SkillUpgradePickupInteractEvent e;

    public static void Trigger(EventStateType type, Weapon weapon, (string, UpgradeSlotType) modifier)
    {
        e.Type = type;
        e.Weapon = weapon;
        e.Modifier = modifier;
        MMEventManager.TriggerEvent(e);
    }
}
