using UnityEngine;

[CreateAssetMenu(fileName = "AgentConfig", menuName = "AI/AgentConfig")]
public class AgentConfigSO : ScriptableObject
{
    public float AttackRange = .5f;
    public float StrafeRange = 1.5f;
    public float ChaseRange = 3f;
    public float MinStrafeDuration = .2f;
    public float MaxStrafeDuration = .5f;
    public float StrafeAngularSpeed = 1.5f;
}
