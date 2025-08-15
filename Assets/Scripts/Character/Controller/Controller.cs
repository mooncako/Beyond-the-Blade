using Sirenix.OdinInspector;
using UnityEngine;


[RequireComponent(typeof(CustomCharacterMovement))]
[RequireComponent(typeof(Targetable))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Animator))]
public class Controller : MonoBehaviour
{
    [field: SerializeField, FoldoutGroup("Base Reference")] public CustomCharacterMovement Movement { get; private set; }  // get / private set is effectively read only
    [field: SerializeField, FoldoutGroup("Base Reference")] public Targetable Targetable { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Health Health { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Vision Vision { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Animator Animator { get; private set; }
    [field: SerializeField, BoxGroup("Stats")] public Stats Stats { get; private set; }

    protected virtual void Awake()
    {
        ApplyStats();
    }

    protected virtual void OnValidate()
    {
        if (Movement == null) Movement = GetComponent<CustomCharacterMovement>();
        if (Targetable == null) Targetable = GetComponent<Targetable>();
        if (Health == null) Health = GetComponent<Health>();
        if (Vision == null) Vision = GetComponent<Vision>();
        if (Animator == null) Animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        
    }

    [Button]
    public void ApplyStats()
    {
        if (Stats == null) return;
        Vision.ApplyStats(Stats);
        Health.ApplyStats(Stats);
    }
}
