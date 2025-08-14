using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _maxHealth;
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _health;

    [SerializeField, BoxGroup("Stats"), ReadOnly] public bool IsAlive => _health > 0;
    [SerializeField, BoxGroup("Stats"), ReadOnly] public float HealthPercentage => _health / _maxHealth;

    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDamage;
    [FoldoutGroup("Events")] public UnityEvent OnHealthRecovery;
    [FoldoutGroup("Events")] public UnityEvent<DamageInfo> OnDeath;

    public void ApplyStats(Stats stats)
    {
        _maxHealth = stats.MaxHealth;
        _health = _maxHealth;
    } 
}
