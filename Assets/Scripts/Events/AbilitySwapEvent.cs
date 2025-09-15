using MoreMountains.Tools;
using Unity.Mathematics;
using UnityEngine;

public struct AbilitySwapEvent
{
    public string SkillId;
    public int Index;

    public AbilitySwapEvent(string skillId, int index)
    {
        SkillId = skillId;
        Index = index;
    }

    public static AbilitySwapEvent e;

    public static void Trigger(string skillId, int index)
    {
        e.SkillId = skillId;
        e.Index = index;
        MMEventManager.TriggerEvent(e);
    }
}
