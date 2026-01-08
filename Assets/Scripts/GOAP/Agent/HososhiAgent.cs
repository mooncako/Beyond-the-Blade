using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

public class HososhiAgent : AgentTypeFactoryBase
{
    public override IAgentTypeConfig Create()
    {
        var factory = new AgentTypeBuilder("Hososhi");

        factory.AddCapability<HososhiCapabilityFactory>();

        return factory.Build();
    }
}
