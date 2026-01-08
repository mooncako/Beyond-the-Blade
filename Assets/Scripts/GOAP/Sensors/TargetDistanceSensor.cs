using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class TargetDistanceSensor : LocalWorldSensorBase
{
    public override void Created()
    {

    }

    public override void Update()
    {

    }

    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        var distanceCheck = references.GetCachedComponent<DistanceCheck>();
        if (distanceCheck == null) return false;

        return Mathf.FloorToInt(distanceCheck.Distance);
    }
}
