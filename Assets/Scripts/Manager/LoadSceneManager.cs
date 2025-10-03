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
    [SerializeField, BoxGroup("Settings")] private float _startStrength = 0;
    [SerializeField, BoxGroup("Settings")] private float _endStrength = 6;
    [SerializeField, BoxGroup("Settings")] private float _duration = .5f;

    private Tween _strengthTween;

    void OnValidate()
    {
        if (_transitionVolume == null)
        {
            _transitionVolume = GetComponent<CustomPassVolume>();
            _transitionVolume.enabled = false;
        }
    }


    void OnEnable()
    {
        this.MMEventStartListening<LoadSceneEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<LoadSceneEvent>();
        _strengthTween.Stop();
        _transitionMaterial.SetFloat("_RealmStrength", _startStrength);
    }

    public void OnMMEvent(LoadSceneEvent e)
    {
        _transitionVolume.enabled = true;
        _strengthTween.Stop();
        LevelTransitionEvent.Trigger(EventStateType.OnEventStart);
        _strengthTween = Tween.Custom(_startStrength, _endStrength, _duration, newVal => _transitionMaterial.SetFloat("_RealmStrength", newVal)).OnComplete(() =>
        {
            SceneManager.LoadScene(e.SceneName);
            Tween.Delay(.1f).OnComplete(() =>
            {
                LevelTransitionEvent.Trigger(EventStateType.OnEventEnd);
                _strengthTween = Tween.Custom(_endStrength, _startStrength, _duration, newVal => _transitionMaterial.SetFloat("_RealmStrength", newVal)).OnComplete(() =>
                {
                    _transitionVolume.enabled = false;
                    
                });
            });
        });
        
    }
}
