using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] protected FMODUnity.EventReference _attackSFX;
    [SerializeField] protected FMODUnity.EventReference _footstepSFX;
    [SerializeField] protected FMODUnity.EventReference _movementSFX;
    [SerializeField] protected FMODUnity.EventReference _onHitSFX;
    [SerializeField] protected FMODUnity.EventReference _onSpawnSFX;
    [SerializeField] protected FMODUnity.EventReference _onDeathSFX;
    [SerializeField] protected FMODUnity.EventReference _dissolveDeathSFX;
    [SerializeField] protected FMODUnity.EventReference _onHealthRecoverySFX    ;

    [Header("References")]
    [SerializeField] protected Health _health;

    void OnValidate()
    {
        if (_health == null) _health = GetComponent<Health>();
    }

    protected virtual void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamage.AddListener(PlayOnHitSFX);
            _health.OnDeath.AddListener(PlayOnDeathSFX);
            _health.OnHealthRecovery.AddListener(PlayOnHealthRecoverySFX);
        }
    }

    protected virtual void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamage.RemoveListener(PlayOnHitSFX);
            _health.OnDeath.RemoveListener(PlayOnDeathSFX);
            _health.OnHealthRecovery.RemoveListener(PlayOnHealthRecoverySFX);
        }
    }

    public virtual void PlayFootstep()
    {

    }

    public virtual void PlayMovementSound()
    {

    }

    public virtual void PlayAttackSFX()
    {

    }

    public virtual void PlayOnHitSFX(DamageInfo info)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_onHitSFX, transform.position);
    }

    public virtual void PlayOnSpawnSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(_onSpawnSFX, transform.position);
    }

    public virtual void PlayOnDeathSFX(DamageInfo info)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_onDeathSFX, transform.position);
    }

    public virtual void PlayDissolveDeathSFX(DamageInfo info)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_dissolveDeathSFX, transform.position);
    }
    public virtual void PlayOnHealthRecoverySFX(float amount)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_onHealthRecoverySFX, transform.position);
    }
}
