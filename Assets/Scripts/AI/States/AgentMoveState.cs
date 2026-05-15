using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentMoveState", menuName = "AI/States/AgentMoveState")]
public class AgentMoveState : AgentStateSO
{
    [SerializeReference, BoxGroup("References")] private IAgentAction _movementAction;

    public override bool CanEnter(Agent agent)
    {
        return agent != null && agent.Blackboard != null && agent.Blackboard.HasTarget;
    }

    public override void OnStateEnter(Agent agent)
    {
        base.OnStateEnter(agent);
        _movementAction?.Execute(agent);
    }

    public override void OnStateExit(Agent agent)
    {
        base.OnStateExit(agent);
        _movementAction?.Stop(agent);
    }

    public override void OnStateUpdate(Agent agent, float deltaTime)
    {
        base.OnStateUpdate(agent, deltaTime);

        if (agent.Blackboard == null || !agent.Blackboard.HasTarget)
        {
            _movementAction?.Stop(agent);
            return;
        }

        _movementAction?.Update(agent, deltaTime);
    }
}
