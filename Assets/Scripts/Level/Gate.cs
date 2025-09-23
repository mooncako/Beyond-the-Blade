using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private string _levelName;
    [SerializeField, BoxGroup("Settings")] private bool _isNewSession;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;

    void OnValidate()
    {
        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (_isNewSession)
            {
                other.GetComponent<PlayerController>().StartNewSession();
            }
            LoadSceneEvent.Trigger(_levelName);
        }
    }
}
