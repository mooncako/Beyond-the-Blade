using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

[GoapId("HeavyAttackEnergySensor")]
public class HeavyAttackEnergySensor : LocalWorldSensorBase
{

    public override void Created()
    {
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
