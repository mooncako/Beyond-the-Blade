using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class PlayerCombatRangeSensor : LocalWorldSensorBase, IInjectable
{

    private bool _isPlayerInCombatRange = false;

    public override void Created()
    {

    }

    public void Inject(DependencyInjector injector)
    {

    }


    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        _isPlayerInCombatRange = references.GetCachedComponent<Brain>().IsPlayerInCombatRange;
        return _isPlayerInCombatRange;
    }

    public override void Update()
    {
    }

}
