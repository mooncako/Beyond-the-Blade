using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillZVolume : MonoBehaviour,
    MMEventListener<ToggleKillzEvent>
{
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField, BoxGroup("Settings")] private LayerMask _killZMask;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _enabled = true;

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

    void OnEnable()
    {
        this.MMEventStartListening<ToggleKillzEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<ToggleKillzEvent>();
    }


    void OnTriggerEnter(Collider other)
    {
        if ((_killZMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(!_enabled) return;
            other.GetComponent<Health>().Damage(new DamageInfo(99999, other.gameObject, other.GetComponent<Health>(), gameObject, DamageType.KillZ), true);
        }
    }

    public void OnMMEvent(ToggleKillzEvent e)
    {
        _enabled = e.Toggle;
    }
}
