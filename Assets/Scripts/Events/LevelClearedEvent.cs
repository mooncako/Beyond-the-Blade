using MoreMountains.Tools;
using UnityEngine;

public struct LevelClearedEvent
{
    public LevelRewardType RewardType;
    public bool TriggerSlowMo;

    public LevelClearedEvent(LevelRewardType rewardType, bool triggerSlowMo = true)
    {
        RewardType = rewardType;
        TriggerSlowMo = triggerSlowMo;
    }


    public static LevelClearedEvent e;
    public static void Trigger(LevelRewardType rewardType, bool triggerSlowMo = true)
    {
        e.RewardType = rewardType;
        e.TriggerSlowMo = triggerSlowMo;
        MMEventManager.TriggerEvent(e);
    }
}
