using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class LanternCapabilityFactory : CapabilityFactory
{
    //TODO: Add new goals to represent different personality

    public override ICapabilityConfig Create()
    {

        var builder = new CapabilityBuilder("Lantern");

        BuildGoals(builder);
        BuildActions(builder);
        BuildSensors(builder);

        return builder.Build();
    }

    protected override void BuildGoals(CapabilityBuilder builder)
    {
        base.BuildGoals(builder);
    }

    protected override void BuildActions(CapabilityBuilder builder)
    {
        base.BuildActions(builder);

        builder.AddAction<AttackAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<PlayerDistance>(Comparison.SmallerThanOrEqual, 1)
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(2f)
            .SetBaseCost(2);
        
        builder.AddAction<FireProjectileAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(20f)
            .SetBaseCost(3);

        builder.AddAction<AttackCautiousAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealthCautious>(EffectType.Decrease)
            .SetStoppingDistance(2f)
            .SetBaseCost(6);

        builder.AddAction<StrafeAction>()
            .SetTarget<StrafeTarget>()
            .AddEffect<IsTargetVisible>(EffectType.Increase)
            .AddEffect<IsStrafe>(EffectType.Increase)
            .SetStoppingDistance(3f)
            .SetBaseCost(4);    
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        base.BuildSensors(builder);
        
        builder.AddWorldSensor<TargetDistanceSensor>()
            .SetKey<PlayerDistance>();
    }


}
