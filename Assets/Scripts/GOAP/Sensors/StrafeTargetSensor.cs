using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.AI;

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
        Vector3 position = GetNextPosition(agent, references.GetCachedComponent<EnemyController>().CurrentTargetTransform.position, references.GetCachedComponent<EnemyController>().Agent);
        if (position != Vector3.zero)
        {
            return new PositionTarget(position);
        }
        else
        {
            return new PositionTarget(agent.Transform.position);
        }
        
    }

    public override void Update()
    {

    }

    private Vector3 GetNextPosition(IActionReceiver agent, Vector3 center, NavMeshAgent navMeshAgent)
    {
        return CircleCalculation.RandomPointOnCircleFromEdgeSafe(center, agent.Transform.position, _strafeSensorConfig.Distance, navMeshAgent, maxAttempts: 40);
    }
}
