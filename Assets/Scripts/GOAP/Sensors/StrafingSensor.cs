using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class StrafingSensor : LocalWorldSensorBase
{
    public override void Created()
    {
        
    }

    public override void Update()
    {

    }

    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        var movement = references.GetCachedComponent<CustomCharacterMovement>();

        return movement.HasPath;
    }
}
