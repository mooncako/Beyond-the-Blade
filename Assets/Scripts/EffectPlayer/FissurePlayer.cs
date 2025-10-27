using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class FissurePlayer : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private ParticleSystem _fissureFront;
    [SerializeField, BoxGroup("References")] private ParticleSystem _fissureBack;
    [SerializeField, BoxGroup("References")] private ParticleSystemRenderer _frontRenderer;
    [SerializeField, BoxGroup("References")] private ParticleSystemRenderer _backRenderer;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _fissureProgressCurve;
    [SerializeField, BoxGroup("Settings")] private float _fissureOpenTime = .8f;
    [SerializeField, BoxGroup("Settings")] private float _colorChangeTime = 2f;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _screenCrackEdgeCurve;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _screenCrackOffsetCurve;
    [SerializeField, BoxGroup("Settings")] private float _maxTwirlStrength = 5.5f;
    [SerializeField, BoxGroup("Settings")] private float _minTwirlStrength = 1.5f;
    [SerializeField, BoxGroup("Settings"), ColorUsage(true, true)] private Color _backMatStartColor;
    [SerializeField, BoxGroup("Settings"), ColorUsage(true, true)] private Color _backMatEndColor;

    private Tween _fissureOpenTween;
    private Tween _screenCrackTween;
    private Tween _backMatColorTween;

    void OnValidate()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [Button]
    private void Play()
    {
        _fissureBack.Stop();
        _fissureFront.Stop();
        _fissureFront.Play();
        _fissureBack.Play();

        _fissureOpenTween.Stop();
        _screenCrackTween.Stop();
        _backMatColorTween.Stop();

        _fissureOpenTween = Tween.Custom(0, 1, duration: _fissureOpenTime, onValueChange: prog => _frontRenderer.material.SetFloat("_FissureProgress", _fissureProgressCurve.Evaluate(prog)));
        _backRenderer.material.SetFloat("_TwirlStrength", Random.Range(_minTwirlStrength, _maxTwirlStrength));
        Tween.Delay(_fissureOpenTime / 2).OnComplete(() =>
        {
            _screenCrackTween = Tween.Custom(0, 1, duration: _fissureOpenTime, onValueChange: prog =>
            {
                _frontRenderer.material.SetFloat("_Edge", _screenCrackEdgeCurve.Evaluate(prog));
                _frontRenderer.material.SetFloat("_Offset", _screenCrackOffsetCurve.Evaluate(prog));
            });
            _backMatColorTween = Tween.Custom(0, 1, duration: _colorChangeTime, onValueChange: prog =>
            {
                _backRenderer.material.SetColor("_VoidColor", Color.Lerp(_backMatStartColor, _backMatEndColor, prog));
            });
        });
        
    }
}
