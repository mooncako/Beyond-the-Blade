using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.UIElements;


public class IdleTargetSensor : LocalTargetSensorBase
{

    // Called when script is initialized
    public override void Created()
    {
        
    }

    // Is called every frame that an agent of an `AgentType` that uses this sensor needs it.
    // This can be used to 'cache' data that is used in the `Sense` method.
    // Eg look up all the trees in the scene, and then find the closest one in the Sense method.
    public override void Update()
    {
        
    }

    public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
    {
        return new PositionTarget(agent.Transform.position);
    }

}
