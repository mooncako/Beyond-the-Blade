using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AgentStateSO : ScriptableObject
{
    [SerializeField, BoxGroup("Settings")] protected List<AgentStateSO> _prohibitedStates = new List<AgentStateSO>();

    public virtual void OnStateEnter()
    {
        
    }

    public virtual void OnStateUpdate(float deltaTime)
    {
        
    }

    public virtual void OnStateExit()
    {
        
    }

    public bool IsStateProhibited(AgentStateSO state)
    {
        return _prohibitedStates.Contains(state);
    }

    
}
