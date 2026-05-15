using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class MoveAction : IAgentAction
{

    private bool _isRunning;

    public event Action OnActionStarted;
    public event Action OnActionEnded;

    public void Execute(Agent agent)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        OnActionStarted?.Invoke();
    }

    public void Stop(Agent agent)
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        OnActionEnded?.Invoke();
    }

    public void Update(Agent agent, float deltaTime)
    {
        if (!_isRunning || agent == null || agent.Blackboard == null)
        {
            return;
        }

        // Vector3 targetPosition = agent.Blackboard.TargetPosition;
        // Vector3 nextPosition = Vector3.MoveTowards(agent.transform.position, targetPosition, _moveSpeed * deltaTime);
        // agent.transform.position = nextPosition;

        // if (Vector3.Distance(agent.transform.position, targetPosition) <= _stoppingDistance)
        // {
        //     Stop(agent);
        // }
    }
}
