using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour,
    MMEventListener<LoadSceneEvent>
{
    [SerializeField, BoxGroup("References")] private CustomPassVolume _transitionVolume;
    [SerializeField, BoxGroup("References")] private Material _transitionMaterial;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _strengthCurve;
    [SerializeField, BoxGroup("Settings")] private float _startStrength = 1;
    [SerializeField, BoxGroup("Settings")] private float _duration = .5f;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isTransitioning = false;
    private Tween _strengthTween;

    void OnValidate()
    {
        if (_transitionVolume == null)
        {
            _transitionVolume = GetComponent<CustomPassVolume>();
            _transitionVolume.enabled = false;
        }
    }

    void Start()
    {
        _strengthTween.Stop();
        _transitionMaterial.SetFloat("_RealmStrength", _startStrength);
    }

    void OnEnable()
    {
        this.MMEventStartListening<LoadSceneEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<LoadSceneEvent>();
    }

    void OnApplicationQuit()
    {
        _strengthTween.Stop();
        _transitionMaterial.SetFloat("_RealmStrength", _startStrength);
    }

    public void OnMMEvent(LoadSceneEvent e)
    {
        if (!_isTransitioning)
        {
            _isTransitioning = true;
            _transitionVolume.enabled = true;
            _strengthTween.Stop();
            LevelTransitionEvent.Trigger(EventStateType.OnEventStart);
            _strengthTween = Tween.Custom(0, 1, _duration, newVal => _transitionMaterial.SetFloat("_RealmStrength", _strengthCurve.Evaluate(newVal))).OnComplete(() =>
            {
                SceneManager.LoadScene(e.SceneName);
                Tween.Delay(_duration).OnComplete(() =>
                {
                    _isTransitioning = false;
                    LevelTransitionEvent.Trigger(EventStateType.OnEventEnd);
                    _strengthTween = Tween.Custom(1, 0, _duration, newVal => _transitionMaterial.SetFloat("_RealmStrength", _strengthCurve.Evaluate(newVal))).OnComplete(() =>
                    {
                        _transitionVolume.enabled = false;

                    });
                });
            });
        }
        
        
    }
}
