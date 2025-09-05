using MoreMountains.Tools;
using UnityEngine;

public class PlayerUI : CharacterUI
{
    [SerializeField] private ResourceBar _eneryBar;
    [SerializeField] private Energy _energy;
    protected override void OnValidate()
    {
        if(_energy == null) _energy = GetComponentInParent<Energy>();
        if (_health == null) _health = GetComponentInParent<Health>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if(_energy != null)
        {
            _energy.OnEnergyGain.AddListener(OnEnergyChanged);
            _energy.OnExecution.AddListener(() => OnEnergyChanged(_energy.GetCurrentEnergyPercentage()));
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



}
