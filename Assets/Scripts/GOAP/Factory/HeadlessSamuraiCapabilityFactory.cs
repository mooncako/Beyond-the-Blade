using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class HeadlessSamuraiCapabilityFactory : CapabilityFactory
{
    //TODO: Add new goals to represent different personality

    public override ICapabilityConfig Create()
    {

        var builder = new CapabilityBuilder("HeadlessSamurai");

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
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(1.5f)
            .SetBaseCost(4);
        
        builder.AddAction<AttackCautiousAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealthCautious>(EffectType.Decrease)
            .SetStoppingDistance(1.5f)
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

        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();

        builder.AddTargetSensor<StrafeTargetSensor>()
            .SetTarget<StrafeTarget>();
    }


}
