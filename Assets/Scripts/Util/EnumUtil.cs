using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnumUtil
{
    public static IEnumerable<LevelRewardType> GetFlags(LevelRewardType value)
    {
        foreach (LevelRewardType flag in Enum.GetValues(typeof(LevelRewardType)))
        {
            if (flag != LevelRewardType.None && flag != LevelRewardType.All && value.HasFlag(flag))
                yield return flag;
        }
    }
}
