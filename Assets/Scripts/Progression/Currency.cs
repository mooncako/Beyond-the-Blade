using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Currency : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SphereCollider _collider;

    protected virtual void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }
    }

}
