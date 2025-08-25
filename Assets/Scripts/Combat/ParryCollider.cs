using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ParryCollider : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SphereCollider _collider;
    [SerializeField, BoxGroup("Settings")] private float _radius = .5f;
    [BoxGroup("Events")] public UnityEvent OnParried;

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

    public void OnParry()
    {
        OnParried.Invoke();
        CloseCollider();
    }
}
