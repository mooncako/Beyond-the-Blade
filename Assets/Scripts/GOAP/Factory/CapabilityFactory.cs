using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class CapabilityFactory : CapabilityFactoryBase
{
    public override ICapabilityConfig Create()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void BuildGoals(CapabilityBuilder builder) { }
    protected virtual void BuildActions(CapabilityBuilder builder) { }
    protected virtual void BuildSensors(CapabilityBuilder builder)
    {
        builder.AddWorldSensor<HeavyAttackEnergySensor>()
            .SetKey<HeavyAttackEnergy>();

        builder.AddWorldSensor<PlayerHealthSensor>()
            .SetKey<PlayerHealth>();
        
        builder.AddWorldSensor<PlayerHealthSensor>()
            .SetKey<PlayerHealthCautious>();

        builder.AddWorldSensor<TargetVisibilitySensor>()
            .SetKey<IsTargetVisible>();

        builder.AddWorldSensor<StrafingSensor>()
            .SetKey<IsStrafe>();
    }
}
