using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class PlayerTargetSensor : LocalTargetSensorBase, IInjectable
{
    private AttackSensorConfigSO _attackSensorConfig;
    private Collider[] _colliders = new Collider[1];

    public override void Created()
    {
    }

    public void Inject(DependencyInjector injector)
    {
        _attackSensorConfig = injector.AttackSensorConfig;
    }

    public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
    {
        if (Physics.OverlapSphereNonAlloc(agent.Transform.position, _attackSensorConfig.SensorRadius, _colliders, _attackSensorConfig.AttackableLayerMask) > 0)
        {
            return new TransformTarget(_colliders[0].transform);
        }

        return null;
    }

    public override void Update()
    {
    }

}
