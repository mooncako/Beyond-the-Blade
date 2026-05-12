using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AgentBlackboard))]
public class Agent : SerializedMonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected AgentBlackboard _blackboard;
    [SerializeField, BoxGroup("Settings")] protected List<IAgentAction> _actions = new List<IAgentAction>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] protected IAgentAction _currentAction;
    protected Queue<IAgentAction> _actionQueue = new Queue<IAgentAction>();

    protected virtual void OnValidate()
    {
        if(_blackboard == null) _blackboard = GetComponent<AgentBlackboard>();
    }



    protected virtual void Update()
    {
        if(_currentAction != null)
        {
            _currentAction.Update(Time.deltaTime);
        }
    }
}
