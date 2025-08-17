using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerSensor : MonoBehaviour
{
    public SphereCollider Collider;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerLayerMask;
    [SerializeField, BoxGroup("References")] private Vision _vision;
    public delegate void PlayerEnterEvent(Transform player);
    public delegate void PlayerExitEvent(Vector3 lastKnownPosition);

    public event PlayerEnterEvent OnPlayerEnter;
    public event PlayerExitEvent OnPlayerExit;


    void OnValidate()
    {
        if (Collider == null) Collider = GetComponent<SphereCollider>();
        if (_vision == null) _vision = GetComponentInParent<Vision>();
    }


    void OnTriggerEnter(Collider other)
    {
        if ((_playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnPlayerEnter?.Invoke(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((_playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}
