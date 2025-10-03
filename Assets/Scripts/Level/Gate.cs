using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour, MMEventListener<GateOpenEvent>
{
    [SerializeField, BoxGroup("Settings")] private string _levelName;
    [SerializeField, BoxGroup("Settings")] private bool _isNewSession;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;
    [SerializeField, BoxGroup("Settings")] private bool _alwaysOn = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isOn = false;

    void OnValidate()
    {
        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    void Start()
    {
        if (_alwaysOn)
        {
            _isOn = true;
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<GateOpenEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<GateOpenEvent>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isOn) return;
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (_isNewSession)
            {
                other.GetComponent<PlayerController>().StartNewSession();
            }
            LoadSceneEvent.Trigger(_levelName);
        }
    }

    public void SetLevelName(string levelName, bool isNewSession = false)
    {
        _levelName = levelName;
        _isNewSession = isNewSession;
    }

    public void OnMMEvent(GateOpenEvent e)
    {
        _isOn = true;
        //Open animation, vfx ...etc
    }
}
