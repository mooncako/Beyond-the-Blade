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
    [FoldoutGroup("Events")] public UnityEvent<float> OnIframe;
    [FoldoutGroup("Events")] public UnityEvent<float> OnHealthRecovery;
    [FoldoutGroup("Events")] public UnityEvent OnMaxHealthUpdated;

    [SerializeField, BoxGroup("Debug"), ReadOnly] public bool IsDamageable = true;

    void OnValidate()
    {
        if (_controller == null) _controller = GetComponent<Controller>();
    } 

    public void ApplyStats(Stats stats)
    {
        _maxHealth = stats.MaxHealth;
        _health = _maxHealth;
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
        Debug.Log($"Healed {amount} health. Current health: {_health}/{_maxHealth}");
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

        if (_health <= 0)
        {
            Death(info);
        }
    }

    public void Death(DamageInfo info)
    {
        if (_controller is EnemyController)
        {
            EnemyDeathEvent.Trigger(info);
        }
        gameObject.SetActive(false);
    }

    [Button, BoxGroup("Debug")]
    public void DebugTakeDamaage()
    {
        _health -= 10;
        OnDamage.Invoke(new DamageInfo(10, gameObject,this,gameObject, DamageType.Regular));
    }
}
