using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelMechanicTrain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Animator _animator;
    [field: SerializeField, BoxGroup("References")] private Collider[] _colliders;

    [HideInInspector] public UnityEvent OnTrainArrived;
    [HideInInspector] public UnityEvent OnGateClosed;

    void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    public void EnableShadow()
    {
        for(int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].gameObject.layer = LayerMask.NameToLayer(LIGHTING.DefaultLayer);
        }
    }

    public void OpenGate()
    {
        _animator.SetTrigger("OpenGate");   
    }

    public void CloseGate()
    {
        _animator.SetTrigger("CloseGate");
    }

    private void TrainArrivalAnimEvent()
    {
        OnTrainArrived?.Invoke();
    }

    private void GateClosedAnimEvent()
    {
        for(int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
        OnGateClosed?.Invoke();
    }
}
