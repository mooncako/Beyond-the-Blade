using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class CentipedeCapabilityFactory : CapabilityFactory
{
    public override ICapabilityConfig Create()
    {
        var builder = new CapabilityBuilder("Centipede");

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
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(1.5f)
            .SetBaseCost(4);
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        base.BuildSensors(builder);

        builder.AddTargetSensor<WanderTargetSensor>()
            .SetTarget<WanderTarget>();

        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();
    }
}
