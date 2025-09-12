using Sirenix.OdinInspector;
using UnityEngine;
using MoreMountains.Tools;

[RequireComponent(typeof(SphereCollider))]
public class AbilityPickup : MonoBehaviour, IPickup
{
    [SerializeField, FoldoutGroup("References")] private SphereCollider _collider;

    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;


    void OnEnable()
    {
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
    }

    void OnValidate()
    {
        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }

        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            _collider.radius = 2f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            NewAbilityEvent.Trigger(EventStateType.OnEventStart);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            NewAbilityEvent.Trigger(EventStateType.OnEventEnd);
        }
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        gameObject.SetActive(false);
    }
}
