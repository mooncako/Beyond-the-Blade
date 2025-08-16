using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class HeadlessSamuraiFactory : AgentTypeFactoryBase
{
    public override IAgentTypeConfig Create()
    {
        var factory = new AgentTypeBuilder("HeadlessSamurai");

        factory.AddCapability<WanderCapabilityFactory>();

        return factory.Build();
    }
}
