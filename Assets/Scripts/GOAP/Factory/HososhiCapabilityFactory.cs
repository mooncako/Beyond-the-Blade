using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;


public class HososhiCapabilityFactory : CapabilityFactory
{


    public override ICapabilityConfig Create()
    {

        var builder = new CapabilityBuilder("Hososhi");

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
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(3)
            .SetBaseCost(4);

        builder.AddAction<AttackCautiousAction>()
            .SetTarget<PlayerTarget>()
            .AddEffect<PlayerHealthCautious>(EffectType.Decrease)
            .SetStoppingDistance(3)
            .SetBaseCost(6);

        builder.AddAction<StrafeAction>()
            .SetTarget<StrafeTarget>()
            .AddEffect<IsStrafe>(EffectType.Increase)
            .SetStoppingDistance(.5f)
            .SetBaseCost(4);    
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        builder.AddTargetSensor<WanderTargetSensor>()
            .SetTarget<WanderTarget>();

        builder.AddTargetSensor<PlayerTargetSensor>()
            .SetTarget<PlayerTarget>();

        builder.AddTargetSensor<StrafeTargetSensor>()
            .SetTarget<StrafeTarget>();
    }


}
