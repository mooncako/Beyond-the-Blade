using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SkillPickup : MonoBehaviour, IPickup
{
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;
    [SerializeField, BoxGroup("Settings")] public string SkillId;

    void OnValidate()
    {
        // if ((_attackableMask & (1 << 8)) == 0)
        // {
        //     _attackableMask |= 1 << 8;
        // }
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
