using Sirenix.OdinInspector;
using UnityEngine;
using MoreMountains.Tools;

[RequireComponent(typeof(SphereCollider))]
public class ModifierPickup : MonoBehaviour, IPickup
{
    [SerializeField, FoldoutGroup("References")] private SphereCollider _collider;

    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;

    [SerializeField, BoxGroup("Settings")] private UpgradeSlotType _slotType;
    [SerializeField, BoxGroup("Settings")] private string _modifierId;

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
            SkillUpgradeEvent.Trigger(EventStateType.OnEventStarted, other.GetComponent<PlayerController>().CurrentWeapon, (_modifierId, _slotType));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            SkillUpgradeEvent.Trigger(EventStateType.OnEventEnded, null, ("", UpgradeSlotType.Start));
        }
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        GateOpenEvent.Trigger(LevelManager.Instance.CurrentLevel);
        gameObject.SetActive(false);
    }

    public void AssignId(string id)
    {
        _modifierId = id;
    }
}
