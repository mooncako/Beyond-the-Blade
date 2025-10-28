using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillZVolume : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField, BoxGroup("Settings")] private LayerMask _killZMask;

    void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        if ((_killZMask & (1 << 7)) == 0)
        {
            _killZMask |= 1 << 7;
        }

        if ((_killZMask & (1 << 8)) == 0)
        {
            _killZMask |= 1 << 8;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((_killZMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.GetComponent<Health>().Damage(new DamageInfo(99999, other.gameObject, other.GetComponent<Health>(), gameObject, DamageType.KillZ), true);
        }
    }


}
