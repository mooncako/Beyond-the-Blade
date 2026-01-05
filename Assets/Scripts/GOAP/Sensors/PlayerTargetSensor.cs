using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class PlayerTargetSensor : LocalTargetSensorBase, IInjectable
{

    public override void Created()
    {
    }

    public void Inject(DependencyInjector injector)
    {

    }

    public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
    {
        // if (Physics.OverlapSphereNonAlloc(agent.Transform.position, _attackSensorConfig.SensorRadius, _colliders, _attackSensorConfig.AttackableLayerMask) > 0)
        // {
        //     return new TransformTarget(_colliders[0].transform);
        // }

        // return null;

        return PlayerBroadcast.Instance.Players[0] != null ? new TransformTarget(PlayerBroadcast.Instance.Players[0].transform) : null;
    }

    public override void Update()
    {
    }

}
