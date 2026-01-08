using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetVisibilitySensor : LocalWorldSensorBase
{
    public override void Created()
    {
        
    }

    public override void Update()
    {

    }

    public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
    {
        Vision vision = references.GetCachedComponentInParent<Vision>();
        if (vision == null) return false;

        return vision.TestTargetVisibility(1);
    }
}
