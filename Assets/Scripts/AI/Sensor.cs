using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sensor : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected SphereCollider _collider;

    void OnValidate()
    {
        if (_collider == null) _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
    }
}
