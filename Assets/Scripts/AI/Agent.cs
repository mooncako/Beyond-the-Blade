using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AgentBlackboard))]
public class Agent : SerializedMonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected AgentBlackboard _blackboard;
    [field: SerializeField, BoxGroup("Settings")] protected List<AgentStateSO> _states = new List<AgentStateSO>();
    [SerializeField, BoxGroup("Settings")] protected AgentStateSO _initialState;
    [SerializeField, BoxGroup("Debug"), ReadOnly] protected AgentStateSO _currentState;
    protected Queue<IAgentAction> _actionQueue = new Queue<IAgentAction>();

    protected virtual void OnValidate()
    {
        if(_blackboard == null) _blackboard = GetComponent<AgentBlackboard>();
    }



    protected virtual void Update()
    {
        if(_currentState != null)
        {
            _currentState.OnStateUpdate(Time.deltaTime);
        }
    }
}
