using UnityEngine;

public readonly struct ModifierContext
{
    public readonly Controller Caster;

    public ModifierContext(Controller caster)
    {
        Caster = caster;
    }
}
