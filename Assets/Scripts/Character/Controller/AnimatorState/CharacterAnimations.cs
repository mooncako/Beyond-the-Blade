using UnityEngine;
using UnityEngine.Animations.Rigging;
using PrimeTween;
using Sirenix.OdinInspector;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CustomCharacterMovement _characterMovement;
    [SerializeField] private AnimationCurve _deflectCurve;
    private int _currentDeflectVar;

    private void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_characterMovement == null) _characterMovement = GetComponent<CustomCharacterMovement>();
    }

    private void Start()
    {
        // add listener to health OnDeath event
        // if (TryGetComponent(out Health health))
        // {
        //     health.OnDeath.AddListener(Death);
        // }
    }

    private void Death(DamageInfo damageInfo)
    {
        // set IsAlive bool
        _animator.SetBool("IsAlive", false);
    }

    private void Update()
    {
        Vector3 worldVelocity = _characterMovement.Velocity;
        Vector3 localVelocity = transform.InverseTransformVector(worldVelocity);
        localVelocity /= _characterMovement.Speed;
    }

    public void ToggleParameter(string parameterName)
    {
        _animator.SetBool(parameterName, true);
    }

    public void EnemyGetHit()
    {
        if(!_animator.GetBool("CanStun"))return;
        _animator.SetBool("GetHit", true);
    


        
    }
    // public void PlayDeflectAnimation()
    // {   if(_isPlayingDeflect) 
    // {
    //         ArmIKRig.weight = 1;
    //         DeflectRig.weight = 1;
    //         _isPlayingDeflect = true;
    // }
    // else{
    //     ArmIKRig.weight = 0;
    //     DeflectRig.weight = 0;
    //     _isPlayingDeflect = false;
    // }

    //     // Tween.Custom(1, 0, duration, onValueChange: newVal => DeflectRig.weight = _deflectCurve.Evaluate(newVal));
    //     // Tween.Custom(1, 0, duration, onValueChange: newVal => ArmIKRig.weight = _deflectCurve.Evaluate(newVal));

    // }

   
    public void PlayDeflectAnimation()
    {
        _animator.SetBool("Deflect", true);
        int deflectVar = Random.Range(0, 4);
        while (deflectVar == _currentDeflectVar)
        {
            deflectVar = Random.Range(0, 4);
        }
        _currentDeflectVar = deflectVar;
        _animator.SetInteger("DeflectVar", _currentDeflectVar);
    }


}