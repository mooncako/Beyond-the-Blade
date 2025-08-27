using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class ParryCollider : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SphereCollider _collider;
    [SerializeField, BoxGroup("Settings")] private float _radius = .5f;
    [BoxGroup("Events")] public UnityEvent<float> OnParried;

    void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = false;
            _collider.radius = _radius;
        }
    }

    void Awake()
    {
        _collider.radius = _radius;
        CloseCollider();
    }

    public void OpenCollider()
    {
        _collider.enabled = true;
    }

    public void CloseCollider()
    {
        _collider.enabled = false;
    }

    public void OnParry(float duration)
    {
        OnParried.Invoke(duration);
        Debug.Log(1);
        CloseCollider();
    }
}
