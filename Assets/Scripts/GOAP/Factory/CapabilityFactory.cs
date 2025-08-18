using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class CapabilityFactory : CapabilityFactoryBase
{
    public override ICapabilityConfig Create()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void BuildGoals(CapabilityBuilder builder) { }
    protected virtual void BuildActions(CapabilityBuilder builder) { }
    protected virtual void BuildSensors(CapabilityBuilder builder) { }
}
