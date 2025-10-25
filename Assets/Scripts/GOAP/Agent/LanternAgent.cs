using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

public class LanternAgent : AgentTypeFactoryBase
{
    public override IAgentTypeConfig Create()
    {
        var factory = new AgentTypeBuilder("Lantern");

        factory.AddCapability<LanternCapabilityFactory>();

        return factory.Build();
    }
}
