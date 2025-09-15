using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerUI : CharacterUI
{
    [BoxGroup("UI"), SerializeField] private ResourceBar _eneryBar;
    [BoxGroup("UI"), SerializeField] private ResourceBar _staminaBar;

    [BoxGroup("Stats"), SerializeField] private Energy _energy;
    [BoxGroup("Stats"), SerializeField] private Stamina _stanima;
    protected override void OnValidate()
    {
        if(_energy == null) _energy = GetComponentInParent<Energy>();
        if (_health == null) _health = GetComponentInParent<Health>();
        if(_stanima == null) _stanima = GetComponentInParent<Stamina>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_energy != null)
        {
            _energy.OnEnergyGain.AddListener(OnEnergyChanged);
            _energy.OnExecution.AddListener(() => OnEnergyChanged(_energy.GetCurrentEnergyPercentage()));
        }
        if (_stanima != null)
        {
            _stanima.OnConsumption.AddListener(OnStaminaChanged);
            _stanima.OnStaminaGain.AddListener(OnStaminaChanged);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if(_energy != null)
        {
            _energy.OnEnergyGain.RemoveListener(OnEnergyChanged);
            _energy.OnExecution.RemoveListener(()=>OnEnergyChanged(_energy.GetCurrentEnergyPercentage()));
        }
    }

    private void OnEnergyChanged(float fillAmount)
    {
        _eneryBar.UpdateFillAmount(_energy.GetCurrentEnergyPercentage());
    }
    private void OnStaminaChanged(float fillAmount)
    {
        _staminaBar.UpdateFillAmount(_stanima.GetCurrentStaminaPercentage());
    }



}
