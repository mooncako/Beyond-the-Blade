using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class HeadlessSamuraiAgent : AgentTypeFactoryBase
{
    public override IAgentTypeConfig Create()
    {
        var factory = new AgentTypeBuilder("HeadlessSamurai");

        factory.AddCapability<HeadlessSamuraiCapabilityFactory>();

        return factory.Build();
    }
}
