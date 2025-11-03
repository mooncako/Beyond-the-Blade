using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

[GoapId("HeavyAttackEnergySensor")]
public class HeavyAttackEnergySensor : LocalWorldSensorBase
{
    private int _energy;

    public override void Created()
    {
        _energy = 0;
    }

    public override void Update()
    {

    }

    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        var energy = references.GetCachedComponent<EnemyController>().Energy;
        if (energy == null)
        {
            return false;
        }

        return Mathf.FloorToInt(energy.GetCurrentEnergy());
    }
}
