using MoreMountains.Tools;
using UnityEngine;

public struct LevelClearedEvent
{
    public LevelRewardType RewardType;
    public LevelClearedEvent(LevelRewardType rewardType)
    {
        RewardType = rewardType;
    }


    public static LevelClearedEvent e;
    public static void Trigger(LevelRewardType rewardType)
    {
        e.RewardType = rewardType;
        MMEventManager.TriggerEvent(e);
    }
}
