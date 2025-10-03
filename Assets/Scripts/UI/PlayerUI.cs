using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerUI : CharacterUI
{
    [BoxGroup("UI"), SerializeField] private ResourceBar _energyBar;
    [BoxGroup("UI"), SerializeField] private ResourceBar _staminaBar;

    [BoxGroup("Stats"), SerializeField] private Energy _energy;
    [BoxGroup("Stats"), SerializeField] private Stamina _stamina;
    protected override void OnValidate()
    {
        if (_energy == null) _energy = GetComponentInParent<Energy>();
        if (_health == null) _health = GetComponentInParent<Health>();
        if (_stamina == null) _stamina = GetComponentInParent<Stamina>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_health != null)
        {
            _health.OnStatsUpdated.AddListener(OnReset);
        }
        if (_energy != null)
        {
            _energy.OnEnergyGain.AddListener(OnEnergyChanged);
            _energy.OnExecution.AddListener(() => OnEnergyChanged(_energy.GetCurrentPercentage()));
            _energy.OnStatsUpdated.AddListener(OnReset);
        }
        if (_stamina != null)
        {
            _stamina.OnConsumption.AddListener(OnStaminaChanged);
            _stamina.OnStaminaGain.AddListener(OnStaminaChanged);
            _stamina.OnStatsUpdated.AddListener(OnReset);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (_health != null)
        {
            _health.OnStatsUpdated.RemoveListener(OnReset);
        }
        if (_energy != null)
            {
                _energy.OnEnergyGain.RemoveListener(OnEnergyChanged);
                _energy.OnExecution.RemoveListener(() => OnEnergyChanged(_energy.GetCurrentPercentage()));
                _energy.OnStatsUpdated.RemoveListener(OnReset);
            }
        if (_stamina != null)
        {
            _stamina.OnConsumption.RemoveListener(OnStaminaChanged);
            _stamina.OnStaminaGain.RemoveListener(OnStaminaChanged);
            _stamina.OnStatsUpdated.RemoveListener(OnReset);
        }
    }

    private void OnEnergyChanged(float fillAmount)
    {
        _energyBar.UpdateFillAmount(_energy.GetCurrentPercentage());
    }
    private void OnStaminaChanged(float fillAmount)
    {
        _staminaBar.UpdateFillAmount(_stamina.GetCurrentPercentage());
    }

    private void OnReset()
    {
        _energyBar.UpdateFillAmount(_energy.GetCurrentPercentage());
        _staminaBar.UpdateFillAmount(_stamina.GetCurrentPercentage());
        _healthBar.UpdateFillAmount(_health.HealthPercentage);
    }



}
