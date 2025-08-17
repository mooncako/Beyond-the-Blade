using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Editor;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class StrafeTargetSensor : LocalTargetSensorBase, IInjectable
{

    private StrafeSensorConfigSO _strafeSensorConfig;

    public override void Created()
    {

    }

    public void Inject(DependencyInjector injector)
    {
        _strafeSensorConfig = injector.StrafeSensorConfig;
    }

    public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
    {
        Vector3 position = GetNextPosition(agent, references.GetCachedComponent<EnemyController>().CurrentTargetTransform.position);
        return new PositionTarget(position);
    }

    public override void Update()
    {

    }

    private Vector3 GetNextPosition(IActionReceiver agent, Vector3 center)
    {
        return CircleCalculation.RandomPointOnCircleFromEdge(center, agent.Transform.position, _strafeSensorConfig.Distance);
    }
}
