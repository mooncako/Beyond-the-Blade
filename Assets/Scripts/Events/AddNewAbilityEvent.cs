using MoreMountains.Tools;
using UnityEngine;

public struct AddNewAbilityEvent
{
    public string SkillId;
    public int Index;

    public AddNewAbilityEvent(string skillId, int index)
    {
        SkillId = skillId;
        Index = index;
    }

    public static AddNewAbilityEvent e;

    public static void Trigger(string skillId, int index)
    {
        e.SkillId = skillId;
        e.Index = index;
        MMEventManager.TriggerEvent(e);
    }
}
