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
    MMEventListener<PlayerOnDamageEvent>
{
    [SerializeField, BoxGroup("References")] private VolumeProfile _profile;
    [SerializeField, BoxGroup("Settings")] private float _startVignetteIntensity = 0;
    [SerializeField, BoxGroup("Settings")] private float _maxVignetteIntensity = .4f;
    [SerializeField, BoxGroup("Settings")] private float _startChromaticAberrationIntensity = 0f;
    [SerializeField, BoxGroup("Settings")] private float _maxChromaticAberrationIntensity = .3f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vignette _vignette;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private ChromaticAberration _chromaticAberration;
    private Tween _vignetteTween;
    private Tween _chromaticAberrationTween;

    void Start()
    {
        if (_profile != null)
        {
            _profile.TryGet(out _vignette);
            _profile.TryGet(out _chromaticAberration);
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<PlayerOnHealthChangeEvent>();
        this.MMEventStartListening<PlayerOnDamageEvent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerOnHealthChangeEvent>();
        this.MMEventStopListening<PlayerOnDamageEvent>();
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

    public void Reset()
    {
        _vignetteTween.Stop();
        if (_vignette != null)
            _vignette.intensity.value = _startVignetteIntensity;
        _chromaticAberrationTween.Stop();
        if (_chromaticAberration != null)
            _chromaticAberration.intensity.value = _startChromaticAberrationIntensity;
    }   
}
