using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeadlessSamuraiBrain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private AgentBehaviour _agent;
    [SerializeField, BoxGroup("References")] private GoapActionProvider _provider;
    [SerializeField, BoxGroup("References")] private GoapBehaviour _goap;

    void OnValidate()
    {
        if (_agent == null) _agent = GetComponent<AgentBehaviour>();
        if (_provider == null) _provider = GetComponent<GoapActionProvider>();
        if (_goap == null) _goap = GetComponent<GoapBehaviour>();
    }

    void Awake()
    {
        if(_provider != null && _goap != null)
        {
            if(_provider.AgentTypeBehaviour == null)
            {
                _provider.AgentType = _goap.GetAgentType("HeadlessSamurai");
            }
        }
    }

    void Start()
    {
        _provider.RequestGoal<WanderGoal>(false);
    }
}
