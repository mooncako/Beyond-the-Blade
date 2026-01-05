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
        base.BuildGoals(builder);
    }

    protected override void BuildActions(CapabilityBuilder builder)
    {
        base.BuildActions(builder);

        builder.AddAction<AttackAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealth>(EffectType.Decrease)
            .SetStoppingDistance(3)
            .SetBaseCost(4);

        builder.AddAction<AttackCautiousAction>()
            .SetTarget<PlayerTarget>()
            .AddCondition<IsTargetVisible>(Comparison.GreaterThanOrEqual, 1)
            .AddEffect<PlayerHealthCautious>(EffectType.Decrease)
            .SetStoppingDistance(3)
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
    }


}
