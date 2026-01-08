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

        builder.AddGoal<KillPlayerGoal>()
            .AddCondition<PlayerHealth>(Comparison.SmallerThanOrEqual, 0);
    }

    protected override void BuildActions(CapabilityBuilder builder)
    {
        builder.AddAction<AttackAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .AddEffect<HeavyAttackEnergy>(EffectType.Increase)
            .SetStoppingDistance(5f)
            .SetBaseCost(4);

        builder.AddAction<HeavyAttackAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddCondition<HeavyAttackEnergy>(Comparison.GreaterThanOrEqual, 9)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(5f)
            .SetBaseCost(3);
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        base.BuildSensors(builder);


        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();
    }
}
