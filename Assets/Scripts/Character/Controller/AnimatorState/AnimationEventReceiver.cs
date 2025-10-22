using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerSoundController _soundController;
    [SerializeField] private AnimationCurve _deathTimeScaleCurve;

    private void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_playerController == null) _playerController = GetComponent<PlayerController>();
        if (_soundController == null) _soundController = GetComponent<PlayerSoundController>();
    }

    // Animation transition windows
    public void OpenParryWindowAnimEvent()
    {
        _playerController.SetActionAvailable(PlayerActionType.Parry, true);
    }

    public void CloseParryWindowAnimEvent()
    {
        _playerController.SetActionAvailable(PlayerActionType.Parry, false);
    }

    public void OpenAttackWindowAnimEvent()
    {
        _playerController.SetActionAvailable(PlayerActionType.Attack, true);
    }

    public void CloseAttackWindowAnimEvent()
    {
        _playerController.SetActionAvailable(PlayerActionType.Attack, false);
    }

    // Combat animation events
    public void FootstepAnimEvent(AnimationEvent animationEvent)
    {
        float animationWeight = animationEvent.animatorClipInfo.weight;
        // Handle footstep sound/effects
    }

    

    public void WeakParryWindowAnimEvent()
    {
        // _combatController.AnimationEventWeakParry();
    }

    public void EndMusoAnimEvent()
    {
        // _combatController.EndMuso();
    }

    // Death animation events
    public void DeathAnimEvent()
    {
        // EventHub.Instance.OnPlayerDeath.Invoke();
    }

    public void DeathTimeScaleAnimEvent()
    {
        Tween.Custom(0, 1, duration: 1.5f, onValueChange: newVal => Time.timeScale = _deathTimeScaleCurve.Evaluate(newVal), useUnscaledTime: true);
    }

    public void StartExecutionLogicAnimEvent()
    {
        _playerController.OnExecutionStarted.Invoke();
    }
}
