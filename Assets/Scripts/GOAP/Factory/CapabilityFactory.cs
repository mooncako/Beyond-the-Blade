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

    protected virtual void BuildGoals(CapabilityBuilder builder)
    {
        builder.AddGoal<ChasePlayerGoal>()
            .AddCondition<IsPlayerInCombatRange>(Comparison.GreaterThanOrEqual, 1);
        
        builder.AddGoal<EvadeGoal>()
            .AddCondition<IsPlayerInCloseRange>(Comparison.GreaterThanOrEqual, 1);

        builder.AddGoal<KillPlayerGoal>()
            .AddCondition<PlayerHealth>(Comparison.SmallerThanOrEqual, 0);

        builder.AddGoal<KillPlayerCautiousGoal>()
            .AddCondition<PlayerHealthCautious>(Comparison.SmallerThanOrEqual, 0);

        builder.AddGoal<StrafeGoal>()
            .AddCondition<IsStrafe>(Comparison.GreaterThanOrEqual, 1);
    }
    protected virtual void BuildActions(CapabilityBuilder builder) 
    {
        builder.AddAction<ChasePlayerAction>()
            .SetTarget<PlayerTarget>()
            .AddEffect<IsPlayerInCombatRange>(EffectType.Increase)
            .SetStoppingDistance(2f)
            .SetBaseCost(2);
    }
    protected virtual void BuildSensors(CapabilityBuilder builder)
    {
        builder.AddWorldSensor<PlayerCombatRangeSensor>()
            .SetKey<IsPlayerInCombatRange>();
        
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
        
        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();

        builder.AddTargetSensor<StrafeTargetSensor>()
            .SetTarget<StrafeTarget>();

        builder.AddTargetSensor<IdleTargetSensor>()
            .SetTarget<IdleTarget>();
        
        builder.AddTargetSensor<EvasiveTargetSensor>()
            .SetTarget<EvasiveTarget>();
    }
}
