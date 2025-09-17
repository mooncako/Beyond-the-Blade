using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SkillPickup : MonoBehaviour, IPickup
{
    [SerializeField, FoldoutGroup("References")] private SphereCollider _collider;

    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;
    [SerializeField, BoxGroup("Settings")] private string _skillId;

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
            PlayerController controller = other.GetComponent<PlayerController>();
            NewSkillEvent.Trigger(EventStateType.OnEventStart, controller, controller.CurrentWeapon.SkillDict[_skillId], _skillId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            NewSkillEvent.Trigger(EventStateType.OnEventEnd, null, null, "");
        }
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        gameObject.SetActive(false);
    }

    public void AssignId(string id)
    {
        _skillId = id;
    }
}
