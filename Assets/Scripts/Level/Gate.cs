using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.VFX;

public class Gate : MonoBehaviour,
    MMEventListener<GateOpenEvent>,
    MMEventListener<GateSplineAdjustEvent>
{
    [SerializeField, BoxGroup("References")] private VisualEffect _portalVFX;
    [SerializeField, BoxGroup("References")] private VisualEffect _toriiGenVFX;
    [SerializeField, BoxGroup("References")] private VisualEffect _toriiGenParticleVFX;
    [SerializeField, BoxGroup("References")] private GameObject _torii;

    [SerializeField, BoxGroup("Settings")] private string _levelName;
    [SerializeField, BoxGroup("Settings")] private bool _isNewSession;
    [SerializeField, BoxGroup("Settings")] private bool _isNewScene = false;
    [SerializeField, BoxGroup("Settings")] private LevelType _levelType;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;
    [SerializeField, BoxGroup("Settings")] private bool _alwaysOn = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isOn = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _gateLevel;
    [SerializeField, BoxGroup("Debug")] private ExitPos _exit;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _targetLevel;

    private Tween _toriiGenTween;
    private Tween _portalTween;

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
        else
        {
            _portalVFX.Stop();
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<GateOpenEvent>();
        this.MMEventStartListening<GateSplineAdjustEvent>();
        _toriiGenVFX.Play();
        
    }

    void OnDisable()
    {
        this.MMEventStopListening<GateOpenEvent>();
        this.MMEventStopListening<GateSplineAdjustEvent>();
        _toriiGenTween.Stop();
        _portalTween.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isOn) return;
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            PlayerController pC = other.GetComponent<PlayerController>();
            if (_isNewSession)
            {
                pC.StartNewSession();
            }

            EnterNewLevelEvent.Trigger(_levelType);

            if(_isNewScene)
            {
                LoadSceneEvent.Trigger(_levelName);
                
            }
            else
            {
                pC.PortalTrigger(_exit.SplineContainer, _exit.GetTeleportExit());
            }
            
        }
    }

    public void SetLevelName(string levelName, LevelType levelType, bool isNewSession = false)
    {
        _levelName = levelName;
        _isNewSession = isNewSession;
        _levelType = levelType;
    }

    public void OnMMEvent(GateOpenEvent e)
    {
        //Open animation, vfx ...etc
        if(e.Level == _gateLevel)
            OpenGate();
    }

    public void OnMMEvent(GateSplineAdjustEvent e)
    {
        if(_targetLevel == null) return;
        if(e.LevelSystem != _targetLevel) return;
        BezierKnot knot = _exit.GetLastKnot();
        knot.Position = _exit.SplineContainer.transform.InverseTransformPoint(_targetLevel.SpawnPos.transform.position);
        _exit.SplineContainer.Spline.SetKnot(_exit.SplineContainer.Spline.Count - 1, knot);
    }

    public void AssignExit(ExitPos exit, LevelSystem gateLevel)
    {
        _exit = exit;
        _gateLevel = gateLevel;
    }

    public void AssignTargetLevel(LevelSystem targetLevel)
    {
        _targetLevel = targetLevel;
    }

    [Button]
    private void OpenGate()
    {
        _toriiGenTween = Tween.Custom(0, 1, duration: .8f, onValueChange: prog =>
        {
            _toriiGenVFX.SetFloat("GenProg", prog);
        }).OnComplete(() =>
        {
            _toriiGenParticleVFX.Play();
            _toriiGenVFX.SetFloat("Alpha", 0);
            _toriiGenVFX.Stop();
            _torii.SetActive(true);
            SetPortalData();
            _portalVFX.Play();
            Tween.Delay(.3f).OnComplete(() =>
            {
                _portalTween = Tween.Custom(0, 1, duration: 1f, onValueChange: prog =>
                {
                    _portalVFX.SetFloat("PortalProg", prog);
                });
            });
            
            _isOn = true;
        });
    }

    private void SetPortalData()
    {
        switch(_levelType)
        {
            case LevelType.Regular:
                _portalVFX.SetInt("Icon", 0);
                _portalVFX.SetVector4("IconColor", PORTALCOLOR.Reguler * 20);
                _portalVFX.SetVector4("VoidColor", PORTALCOLOR.Reguler * 3.4f);
                break;
            case LevelType.Shop:
                _portalVFX.SetInt("Icon", 1);
                _portalVFX.SetVector4("IconColor", PORTALCOLOR.Shop * 20);
                _portalVFX.SetVector4("VoidColor", PORTALCOLOR.Shop * 3.4f);
                break;
            case LevelType.Recover:
                _portalVFX.SetInt("Icon", 2);
                _portalVFX.SetVector4("IconColor", PORTALCOLOR.Recover * 20);
                _portalVFX.SetVector4("VoidColor", PORTALCOLOR.Recover * 3.4f);
                break;
            case LevelType.Boss:
                _portalVFX.SetInt("Icon", 3);
                _portalVFX.SetVector4("IconColor", PORTALCOLOR.Boss * 20);
                _portalVFX.SetVector4("VoidColor", PORTALCOLOR.Boss * 3.4f);
                break;
        }
    }
}
