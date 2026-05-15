using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AgentStateSO : SerializedScriptableObject
{
    [SerializeField, BoxGroup("Settings")] protected List<AgentStateSO> _prohibitedStates = new List<AgentStateSO>();

    public virtual bool CanEnter(Agent agent)
    {
        return true;
    }

    public virtual void OnStateEnter(Agent agent)
    {
    }

    public virtual void OnStateUpdate(Agent agent, float deltaTime)
    {
    }

    public virtual void OnStateExit(Agent agent)
    {
    }

    public bool IsStateProhibited(AgentStateSO state)
    {
        return _prohibitedStates.Contains(state);
    }
}
