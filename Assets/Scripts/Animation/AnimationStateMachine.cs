using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimationStateMachine : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _animancer;
    [SerializeField] private AnimationState _idleState;
    [SerializeField] private AnimationState _moveState;
    [SerializeField] private AnimationState _actionState;
    [ReadOnly] public AnimationState CurrentState;
    [ReadOnly] public AnimationState PreviousState;

    void OnValidate()
    {
        if (_animancer == null)
        {
            _animancer = GetComponent<AnimancerComponent>();
            _idleState = new AnimationState(this, _animancer);
            _moveState = new AnimationState(this, _animancer);
            _actionState = new AnimationState(this, _animancer);
        }
    }

    void Awake()
    {
        CurrentState = _idleState;
    }

    void OnEnable()
    {
        CurrentState = _idleState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
