using System;
using UnityEngine;

[Flags]
public enum LevelRewardType
{
    None = 0,
    Gold = 1 << 0,
    Skill = 1 << 1,
    All = ~0
}
