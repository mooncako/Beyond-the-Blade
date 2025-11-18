using CrashKonijn.Goap.Core;
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
        base.BuildGoals(builder);
    }

    protected override void BuildActions(CapabilityBuilder builder)
    {
        base.BuildActions(builder);
    }

    protected override void BuildSensors(CapabilityBuilder builder)
    {
        base.BuildSensors(builder);
    }
}
