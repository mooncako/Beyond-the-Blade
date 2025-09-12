using MoreMountains.Tools;
using UnityEngine;

public struct NewAbilityCallbackEvent
{
    public string SkillId;
    public int Index;
    public bool IsSuccessfullyAdded;

    public NewAbilityCallbackEvent(string skillId, int index, bool isSuccessfullyAdded)
    {
        SkillId = skillId;
        Index = index;
        IsSuccessfullyAdded = isSuccessfullyAdded;
    }

    public static NewAbilityCallbackEvent e;

    public static void Trigger(string skillId, int index, bool isSuccessfullyAdded)
    {
        e.SkillId = skillId;
        e.Index = index;
        e.IsSuccessfullyAdded = isSuccessfullyAdded;
        MMEventManager.TriggerEvent(e);
    }
}
