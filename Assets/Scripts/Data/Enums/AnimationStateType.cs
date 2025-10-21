using System;
using Unity.VisualScripting;

[Flags]
public enum AnimationStateType
{
    None = 0,
    Idle = 1 << 0,
    Move = 1 << 1,
    Attack = 1 << 2,
    Parry = 1 << 3,
    Ability = 1 << 4,
    Execution = 1 << 5,
    Stagger = 1 << 6,
    Dash = 1 << 7,
    Death = 1 << 8,
    All = ~0

}