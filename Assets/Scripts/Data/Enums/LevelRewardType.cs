using System;
using UnityEngine;

[Flags]
public enum LevelRewardType
{
    None = 0,
    Modifier = 1 << 0,
    Attack = 1 << 1,
    Ability = 1 << 2,
    All = ~0
}
