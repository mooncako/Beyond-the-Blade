using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelMechanicTrain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Animator _animator;

    [HideInInspector] public UnityEvent OnTrainArrived;

    void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        _animator.SetTrigger("OpenGate");
        
    }

    private void TrainArrivalAnimEvent()
    {
        OnTrainArrived?.Invoke();
    }
}
