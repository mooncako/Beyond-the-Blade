using System;
using Animancer;
using Animancer.Units;
using UnityEngine;
using Animancer.TransitionLibraries;
using Sirenix.OdinInspector;
[Serializable]
public class MoveAnimationState : AnimationState
{
    [Header("Movement Mode")]
    [SerializeField] private bool _isDirectionalMovement = false;
    
    [Header("Directional Movement Settings")]
    [SerializeField, ShowIf("_isDirectionalMovement"), Seconds] private float _parameterSmoothTime = 0.15f;
    [SerializeField, ShowIf("_isDirectionalMovement"), Meters] private float _stopProximity = 0.1f;

    [Header("Movement Reference")]
    [SerializeField, ShowIf("_isDirectionalMovement")] private CustomCharacterMovement _characterMovement;
    public ClipTransition UpperBodyClip;
    // Directional movement components
    private SmoothedVector2Parameter _smoothedParameter;
    private MixerTransition2D _mixerTransition;
    private Vector2MixerState _mixerState;

    // Properties
    public bool IsDirectionalMovement 
    { 
        get => _isDirectionalMovement; 
        set => _isDirectionalMovement = value; 
    }

    public MoveAnimationState()
    {
    }

    public MoveAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, ClipTransition clip)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Move";
        Clip = clip;
        _isDirectionalMovement = false;
    }

    public MoveAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, 
        ClipTransition clip, bool isDirectional)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Move";
        Clip = clip;
        _isDirectionalMovement = isDirectional;
        
    }

    public override void OnEnterState()
    {
        // Handle upper body layer
        if (UpperBodyClip != null && _stateMachine.IsHumanoid)
        {
            
            _stateMachine.UpperBodyLayer.Weight = 1;
            _stateMachine.UpperBodyLayer.Play(UpperBodyClip, 0.1f);
        }
        if (_isDirectionalMovement)
        {
            EnterDirectionalMovement();
        }
        else
        {
            // Play base layer (full body or lower body locomotion)
            _stateMachine.BaseLayer.Play(Clip);
        }
    }

    private void EnterDirectionalMovement()
    {
        // Get directional animations from the state machine
        var directionalAnimations = _stateMachine.GetDirectionalMovementAnimations();
        
        if (directionalAnimations == null)
        {
            return;
        }

        // Initialize mixer if needed
        if (_mixerTransition == null)
        {
            _mixerTransition = directionalAnimations.CreateDirectionalMixer();
            FadeDuration = directionalAnimations.FadeDuration;
        }

        // Play the directional mixer
        _mixerState = _animancer.Play(_mixerTransition) as Vector2MixerState;
        AnimancerState = _mixerState;

        // Initialize smoothed parameters using the mixer's parameter names
        if (_smoothedParameter == null)
        {
            _smoothedParameter = new SmoothedVector2Parameter(
                _animancer,
                _mixerTransition.ParameterNameX,  // "Right"
                _mixerTransition.ParameterNameY,  // "Forward" 
                _parameterSmoothTime);
        }

        // Get movement component reference if not assigned
        if (_characterMovement == null && Owner != null)
        {
            _characterMovement = Owner.Movement;
        }
    }

    public override void OnExitState()
    {
        base.OnExitState();
        if (_stateMachine.UpperBodyLayer.Weight > 0)
        {
            _stateMachine.UpperBodyLayer.StartFade(0, 0.15f);
        }
        
        if (_isDirectionalMovement)
        {
            // Clean up smoothed parameter
            if (_smoothedParameter != null)
            {
                _smoothedParameter.Dispose();
                _smoothedParameter = null;
            }
        }
        
        if(Owner != null)
            Owner.Movement.Stop();
    }

    public override void OnInterrupt()
    {
        base.OnInterrupt();
    }

    /// <summary>
    /// Updates the directional mixer parameters based on movement velocity.
    /// Called by the AnimationStateMachine when in directional movement mode.
    /// </summary>
    public void UpdateMovementParameters()
    {
        if (!_isDirectionalMovement || _smoothedParameter == null || _characterMovement == null)
            return;

        Vector3 movementDirection = GetMovementDirection();
        Vector3 localDirection = _stateMachine.transform.InverseTransformDirection(movementDirection);
        
        // Set the target parameter values
        _smoothedParameter.TargetValue = new Vector2(localDirection.x, localDirection.z);
    }

    private Vector3 GetMovementDirection()
    {
        if (_characterMovement == null)
            return Vector3.zero;

        Vector3 velocity = _characterMovement.Velocity;
        
        // Check if we're moving fast enough to avoid jittering near zero velocity
        float squaredMagnitude = velocity.sqrMagnitude;
        if (squaredMagnitude <= _stopProximity * _stopProximity)
        {
            return Vector3.zero;
        }

        // Return normalized direction
        return velocity.normalized;
    }

    /// <summary>
    /// Sets the character movement reference for directional movement
    /// </summary>
    public void SetCharacterMovement(CustomCharacterMovement characterMovement)
    {
        _characterMovement = characterMovement;
    }

    /// <summary>
    /// Gets the current mixer parameter values for debugging (directional mode only)
    /// </summary>
    public Vector2 GetCurrentParameters()
    {
        if (_isDirectionalMovement && _smoothedParameter != null)
            return _smoothedParameter.TargetValue;
        return Vector2.zero;
    }

    /// <summary>
    /// Switches between single clip and directional movement modes
    /// </summary>
    public void SetDirectionalMovement(bool isDirectional)
    {
        if (_isDirectionalMovement == isDirectional)
            return;

        _isDirectionalMovement = isDirectional;
        
        // If we're currently active, re-enter the state to apply changes
        if (_stateMachine.IsInMoveState())
        {
            OnExitState();
            OnEnterState();
        }
    }

#if UNITY_EDITOR
    [Button("Toggle Directional Movement")]
    private void ToggleDirectionalMovement()
    {
        SetDirectionalMovement(!_isDirectionalMovement);
    }

    [Button("Test Parameter Update"), ShowIf("_isDirectionalMovement")]
    private void TestParameterUpdate()
    {
        if (Application.isPlaying)
            UpdateMovementParameters();
    }
#endif
}
