using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] protected Health _health;
    [SerializeField] private ResourceBar _healthBar;

    protected virtual void OnValidate()
    {
        if (_healthBar == null) _healthBar = GetComponentInChildren<ResourceBar>();
        if(_health == null) _health = GetComponentInParent<Health>();
    }

    protected virtual void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamage.AddListener(OnDamage);
        }
    }
    protected virtual void OnDisable()
    {
        _health.OnDamage.RemoveListener(OnDamage);
    }
    private void OnDamage(DamageInfo damageInfo)
    {
        _healthBar.UpdateFillAmount(_health.HealthPercentage);
    }
}
