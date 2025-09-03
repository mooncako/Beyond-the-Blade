using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] protected Health _health;
    private HealthBar _healthBar;
    void Awake()
    {
          _healthBar = GetComponentInChildren<HealthBar>();
        if(_health == null) _health = GetComponentInParent<Health>();
    }


    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamage.AddListener(OnDamage);
        }
    }
    private void OnDisable()
    {
        _health.OnDamage.RemoveListener(OnDamage);
    }
    private void OnDamage(DamageInfo damageInfo)
    {
        _healthBar.OnDamage(_health.HealthPercentage);
    }
}
