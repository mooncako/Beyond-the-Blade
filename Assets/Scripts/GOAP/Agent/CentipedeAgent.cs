using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class CentipedeAgent : AgentTypeFactoryBase
{
    public override IAgentTypeConfig Create()
    {
        var factory = new AgentTypeBuilder("Centipede");

        factory.AddCapability<CentipedeCapabilityFactory>();

        return factory.Build();
    }
}
