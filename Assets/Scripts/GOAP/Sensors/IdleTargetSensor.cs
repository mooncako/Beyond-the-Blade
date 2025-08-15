using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.UIElements;


public class IdleTargetSensor : LocalTargetSensorBase
{
    private static readonly Bounds Bounds = new(Vector3.zero, new Vector3(15, 0, 8));

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
        var random = this.GetRandomPosition(agent);

        // If the existing target is a 'PositionTarget', we can resuse it and just update the position.
        if(existingTarget is PositionTarget positionTarget)
        {
            return positionTarget.SetPosition(random);
        }

        return new PositionTarget(random);
    }

    


    private Vector3 GetRandomPosition(IActionReceiver agent)
    {
        var random = Random.insideUnitCircle * 3f;
        var position = agent.Transform.position + new Vector3(random.x, 0f, random.y);

        if(Bounds.Contains(position))
            return position;
        
        return Bounds.ClosestPoint(position);
    }
}
