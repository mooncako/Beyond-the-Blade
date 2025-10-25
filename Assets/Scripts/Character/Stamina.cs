using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

public class Stamina : MonoBehaviour
{
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _maxStamina;
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _stamina = 10;
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _staminaRegenRate;

    [ShowInInspector, BoxGroup("Stats"), ReadOnly] public bool IsFull => _stamina.Approx(_maxStamina);
    [ShowInInspector, BoxGroup("Stats"), ReadOnly] public bool IsEmpty => _stamina <= 0;
    [FoldoutGroup("Events")] public UnityEvent<float> OnConsumption;
    [FoldoutGroup("Events")] public UnityEvent<float> OnStaminaGain;
    [FoldoutGroup("Events")] public UnityEvent OnStaminaEmpty;
    [FoldoutGroup("Events")] public UnityEvent OnStaminaFull;
    [FoldoutGroup("Events")] public UnityEvent OnStatsUpdated;



     void Update()
    {
        if(_stamina < _maxStamina)
        {
            float oldStamina = _stamina;
            _stamina = Mathf.Min(_stamina + _staminaRegenRate * Time.deltaTime, _maxStamina);
            if (_stamina > oldStamina)
            {
                OnStaminaGain.Invoke(_stamina - oldStamina);
            }
        }
    }
    public void ApplyStats(Stats stats)
    {
        _maxStamina = stats.MaxStamina;
        _staminaRegenRate = stats.StaminaRegeneration;
        _stamina = _maxStamina;
        OnStatsUpdated.Invoke();
    }

    public bool ConsumeStamina(float amount)
    {
        if(_stamina < amount) return false;

        _stamina = Mathf.Clamp(_stamina - amount, 0, _maxStamina);
        OnConsumption.Invoke(amount);
        return true;
    }

    public bool CanConsumeStamina(float amount)
    {
        return _stamina >= amount;
    }
    public float GetCurrentPercentage()
    {
        return _stamina / _maxStamina;
    }


    [Button, FoldoutGroup("Debug")]
    public void TestConsumeStamina()
    {
               ConsumeStamina(2);
    }



    
}
