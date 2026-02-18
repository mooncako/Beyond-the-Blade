using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class PlayerCloseRangeSensor : LocalWorldSensorBase
{
    public override void Created()
    {
        
    }

    public override void Update()
    {
        
    }

    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        return references.GetCachedComponent<Brain>().IsPlayerInCloseRange;
    }
}
