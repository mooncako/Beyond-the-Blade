using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

public class Energy : MonoBehaviour
{
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _maxEnergy;
    [field: SerializeField, BoxGroup("Stats"), ReadOnly] private float _energy = 0;

    [ShowInInspector, BoxGroup("Stats"), ReadOnly] public bool IsFull => _energy.Approx(_maxEnergy);

    [FoldoutGroup("Events")] public UnityEvent OnExecution;
    [FoldoutGroup("Events")] public UnityEvent<float> OnEnergyGain;
    [FoldoutGroup("Events")] public UnityEvent OnStatsUpdated;

    public void ApplyStats(Stats stats)
    {
        _maxEnergy = stats.MaxEnergy;
        OnStatsUpdated.Invoke();
    }

    public void Execute()
    {
        _energy = 0;
        OnExecution.Invoke();
    }
    public float GetCurrentPercentage()
    {
        return _energy / _maxEnergy;
    }

    public float GetCurrentEnergy()
    {
        return _energy;
    }

    public void GainEnergy(float energy)
    {
        _energy = Mathf.Clamp(_energy + energy, 0, _maxEnergy);
        OnEnergyGain.Invoke(energy);
    }

    public void DepletesEnergy(float energy)
    {
        _energy = Mathf.Clamp(_energy - energy, 0, _maxEnergy);
    }
}
