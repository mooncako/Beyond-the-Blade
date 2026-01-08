using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(DontDestroy))]
public class VolumeFeedbackController : MonoBehaviour,
    MMEventListener<PlayerOnHealthChangeEvent>,
    MMEventListener<PlayerOnDamageEvent>,
    MMEventListener<PlayerInitializedEvent>
{
    [SerializeField, BoxGroup("References")] private VolumeProfile _profile;
    [SerializeField, BoxGroup("Settings")] private float _startVignetteIntensity = 0;
    [SerializeField, BoxGroup("Settings")] private float _maxVignetteIntensity = .4f;
    [SerializeField, BoxGroup("Settings")] private float _startChromaticAberrationIntensity = 0f;
    [SerializeField, BoxGroup("Settings")] private float _maxChromaticAberrationIntensity = .3f;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _outlineDistortionCurve;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vignette _vignette;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private ChromaticAberration _chromaticAberration;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private DistortedOutlineSettings _distortedOutline;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private PlayerController _pC;
    private Tween _vignetteTween;
    private Tween _chromaticAberrationTween;
    private Tween _distortedOutlineTween;

    void Start()
    {
        if (_profile != null)
        {
            _profile.TryGet(out _vignette);
            _profile.TryGet(out _chromaticAberration);
            _profile.TryGet(out _distortedOutline);
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<PlayerOnHealthChangeEvent>();
        this.MMEventStartListening<PlayerOnDamageEvent>();
        this.MMEventStartListening<PlayerInitializedEvent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerOnHealthChangeEvent>();
        this.MMEventStopListening<PlayerOnDamageEvent>();
        this.MMEventStopListening<PlayerInitializedEvent>();
        if(_pC != null)
        {
            _pC.Energy.OnEnergyGain.RemoveListener(OnEnergyGain);
            _pC.Energy.OnExecution.RemoveListener(OnExecution);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Reset();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnApplicationQuit()
    {
        Reset();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "TestHub")
        {
            Reset();
        }
    }

    private void OnHealthChange(Health health)
    {
        _vignetteTween.Stop();
        if (_vignette != null)
            _vignetteTween = Tween.Custom(_vignette.intensity.value, Mathf.Lerp(_maxVignetteIntensity, _startVignetteIntensity, health.HealthPercentage), duration: .2f, onValueChange: intensity => _vignette.intensity.value = intensity);

    }

    private void OnDamage(Health health)
    {
        _chromaticAberrationTween.Stop();
        if(_chromaticAberration != null)
        {
            _chromaticAberrationTween = Tween.Custom(_chromaticAberration.intensity.value, _maxChromaticAberrationIntensity, duration: .1f, onValueChange: intensity => _chromaticAberration.intensity.value = intensity).OnComplete(() =>
            {
                _chromaticAberrationTween = Tween.Custom(_chromaticAberration.intensity.value, _startChromaticAberrationIntensity, duration: .2f, onValueChange: intensity => _chromaticAberration.intensity.value = intensity);
            });
        }
    }

    public void OnMMEvent(PlayerOnHealthChangeEvent e)
    {
        OnHealthChange(e.Health);
    }

    public void OnMMEvent(PlayerOnDamageEvent e)
    {
        OnDamage(e.Health);
    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _pC = e.Player;
        _pC.Energy.OnEnergyGain.AddListener(OnEnergyGain);
        _pC.Energy.OnExecution.AddListener(OnExecution);
    }

    private void OnEnergyGain(float gain)
    {
        _distortedOutlineTween.Stop();
        _distortedOutlineTween = Tween.Custom(0, _pC.Energy.EnergyPercentage, .3f, onValueChange: newVal => _distortedOutline.distort.value = _outlineDistortionCurve.Evaluate(newVal));
    }

    private void OnExecution()
    {
        _distortedOutlineTween.Stop();
        float currentVal = _distortedOutline.distort.value;
        _distortedOutlineTween = Tween.Custom(currentVal, 0, .5f, onValueChange: newVal => _distortedOutline.distort.value = newVal);
    }

    public void Reset()
    {
        _vignetteTween.Stop();
        if (_vignette != null)
            _vignette.intensity.value = _startVignetteIntensity;
        _chromaticAberrationTween.Stop();
        if (_chromaticAberration != null)
            _chromaticAberration.intensity.value = _startChromaticAberrationIntensity;
        _distortedOutlineTween.Stop();
        if (_distortedOutline != null)
            _distortedOutline.distort.value = 0;
    }

    
}
