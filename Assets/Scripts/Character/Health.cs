using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Controller _controller;

    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _maxHealth;
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _health;

    [SerializeField, BoxGroup("Stats"), ReadOnly] public bool IsAlive => _health > 0;
    [SerializeField, BoxGroup("Stats"), ReadOnly] public float HealthPercentage => _health / _maxHealth;

    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDamage;
    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDeath;
    [FoldoutGroup("Events")] public UnityEvent<float> OnIframe;
    [FoldoutGroup("Events")] public UnityEvent<float> OnHealthRecovery;
    [FoldoutGroup("Events")] public UnityEvent OnMaxHealthUpdated;
    [FoldoutGroup("Events")] public UnityEvent OnStatsUpdated;

    [SerializeField, BoxGroup("Debug"), ReadOnly] public bool IsDamageable = true;

    void OnValidate()
    {
        if (_controller == null) _controller = GetComponent<Controller>();
    }

    public void ApplyStats(Stats stats)
    {
        _maxHealth = stats.MaxHealth;
        _health = _maxHealth;
        OnStatsUpdated.Invoke();
    }

    public void UpdateMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        OnMaxHealthUpdated.Invoke();
    }

    public void Heal(float amount)
    {
        _health = Mathf.Min(_health + amount, _maxHealth);
        OnHealthRecovery.Invoke(amount);
        PlayerOnHealthChangeEvent.Trigger(this);
    }

    public void Damage(DamageInfo info)
    {
        if (!IsDamageable)
        {
            if (_controller is PlayerController p)
            {
                p.Energy.GainEnergy(p.Stats.DashEnergyGain * p.Stats.ResourceGainMultiplier);
            }
            return;
        }
        _health -= info.Amount * (1 - _controller.Stats.DamageReduction);
        OnDamage.Invoke(info);

        if (_controller is PlayerController)
        {
            PlayerOnHealthChangeEvent.Trigger(this);
            PlayerOnDamageEvent.Trigger(this);
        }

        if (_health <= 0)
        {
            Death(info);
        }
    }

    public void Death(DamageInfo info)
    {
        OnDeath.Invoke(info);
        
        // Trigger death animation state
        if (_controller != null && _controller.AnimationStateMachine != null)
        {
            _controller.AnimationStateMachine.SwitchState(AnimationStateType.Death);
        }
        
        // Handle death logic based on controller type
        if (_controller is EnemyController)
        {
            EnemyDeathEvent.Trigger(info);
            gameObject.SetActive(false);
        }
        else if (_controller is PlayerController player)
        {
            //TODO: Reset player stats
        }
    }

    [Button, BoxGroup("Debug")]
    public void DebugTakeDamaage()
    {
        _health -= 10;
        OnDamage.Invoke(new DamageInfo(10, gameObject,this,gameObject, DamageType.Regular));
    }
}
