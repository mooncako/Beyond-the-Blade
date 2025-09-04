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

    public void ApplyStats(Stats stats)
    {
        _maxEnergy = stats.MaxEnergy;
        _energy = 0;
    }

    public void Execute()
    {
        _energy = 0;
        OnExecution.Invoke();
    }
}
