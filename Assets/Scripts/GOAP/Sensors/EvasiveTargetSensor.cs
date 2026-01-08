using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class EvasiveTargetSensor : LocalTargetSensorBase
{
    public override void Created()
    {
        
    }

    public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
    {
        float evasionMultiplier = Random.Range(1f, 3f);
        Vector3 directionAwayFromPlayer = (agent.Transform.position - PlayerBroadcast.Instance.Players[0].transform.position).normalized;
        Vector3 evasivePosition = agent.Transform.position + directionAwayFromPlayer * evasionMultiplier;
        if(NavMesh.SamplePosition(evasivePosition, out NavMeshHit hit, 1, NavMesh.AllAreas))
        {
            return new PositionTarget(hit.position);
        }
        else
        {
            return new PositionTarget(agent.Transform.position);
        }
    }

    public override void Update()
    {
        
    }
}
