using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(DontDestroy))]
public class VolumeFeedbackController : MonoBehaviour,
    MMEventListener<PlayerOnHealthChangeEvent>
{
    [SerializeField, BoxGroup("References")] private VolumeProfile _profile;
    [SerializeField, BoxGroup("Settings")] private float _startVignetteIntensity = 0;
    [SerializeField, BoxGroup("Settings")] private float _maxVignetteIntensity = .4f;
    [SerializeField] private Vignette _vignette;
    [SerializeField] private ChromaticAberration _chromaticAberration;
    private Tween _vignetteTween;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerOnHealthChangeEvent>();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Reset();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void OnMMEvent(PlayerOnHealthChangeEvent e)
    {
        OnHealthChange(e.Health);
    }

    public void Reset()
    {
        _vignetteTween.Stop();
        if(_vignette != null)
            _vignette.intensity.value = _startVignetteIntensity;
    }   
}
