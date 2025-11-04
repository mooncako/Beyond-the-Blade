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
        builder.AddGoal<WanderGoal>()
            .AddCondition<IsWander>(Comparison.GreaterThanOrEqual, 1);

        builder.AddGoal<KillPlayerGoal>()
            .AddCondition<PlayerHealth>(Comparison.SmallerThanOrEqual, 0);

        builder.AddGoal<KillPlayerCautiousGoal>()
            .AddCondition<PlayerHealthCautious>(Comparison.SmallerThanOrEqual, 0);

        builder.AddGoal<StrafeGoal>()
            .AddCondition<IsStrafe>(Comparison.GreaterThanOrEqual, 1);
    }

    protected override void BuildActions(CapabilityBuilder builder)
    {
        builder.AddAction<WanderAction>()
            .SetTarget<WanderTarget>()
            .AddEffect<IsWander>(EffectType.Increase)
            .SetStoppingDistance(1)
            .SetBaseCost(5);

        builder.AddAction<AttackAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<HeavyAttackEnergy>(Comparison.GreaterThanOrEqual, 5)
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<HeavyAttackEnergy>(EffectType.Decrease)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(2f)
            .SetBaseCost(2);
        
        builder.AddAction<FireProjectileAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .AddEffect<HeavyAttackEnergy>(EffectType.Increase)
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
            .SetStoppingDistance(.5f)
            .SetBaseCost(4);    
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        base.BuildSensors(builder);

        builder.AddTargetSensor<WanderTargetSensor>()
            .SetTarget<WanderTarget>();

        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();

        builder.AddTargetSensor<StrafeTargetSensor>()
            .SetTarget<StrafeTarget>();

        
    }


}
