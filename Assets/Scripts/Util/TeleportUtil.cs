using UnityEngine;

public static class TransformUtil
{
    public static void Teleport(Transform agent, Transform target)
    {
        if(agent.TryGetComponent(out Rigidbody rb))
        {
            agent.position = target.position;
            agent.rotation = target.rotation;
            rb.position = target.position;
        }
        else
        {
            agent.position = target.position;
            agent.rotation = target.rotation;
        }
    }

    public static void Teleport(Transform agent, Vector3 target, Quaternion rotation)
    {
        if (agent.TryGetComponent(out Rigidbody rb))
        {
            agent.position = target;
            agent.rotation = rotation;
            rb.position = target;
        }
        else
        {
            agent.position = target;
            agent.rotation = rotation;
        }
    }

}
