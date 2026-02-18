using MoreMountains.Tools;
using UnityEngine;

public struct SpawnRewardEvent
{
    public LevelRewardType RewardType;
    public SpawnRewardEvent(LevelRewardType rewardType)
    {
        RewardType = rewardType;
    }

    public static SpawnRewardEvent e;
    public static void Trigger(LevelRewardType rewardType)
    {
        e.RewardType = rewardType;
        MMEventManager.TriggerEvent(e);
    }
}
