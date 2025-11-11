using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class FissurePlayer : MonoBehaviour,
    MMEventListener<ProgressionCanvasCloseEvent>
{
    [SerializeField, BoxGroup("References")] private ParticleSystem _fissureFront;
    [SerializeField, BoxGroup("References")] private ParticleSystem _fissureBack;
    [SerializeField, BoxGroup("References")] private ParticleSystem _fissureCrack;
    [SerializeField, BoxGroup("References")] private ParticleSystemRenderer _frontRenderer;
    [SerializeField, BoxGroup("References")] private ParticleSystemRenderer _backRenderer;
    [SerializeField, BoxGroup("References")] private ParticleSystemRenderer _crackRenderer;
    [SerializeField, BoxGroup("References")] private VisualEffect _distortion;
    [SerializeField, BoxGroup("References")] private VisualEffect _fracture;


    [SerializeField, BoxGroup("Settings")] private AnimationCurve _fissureProgressCurve;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _crackProgressCurve;
    [SerializeField, BoxGroup("Settings")] private float _fissureOpenTime = .8f;
    [SerializeField, BoxGroup("Settings")] private float _fissureCloseTime = 1.5f;
    [SerializeField, BoxGroup("Settings")] private float _colorChangeTime = 2f;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _screenCrackEdgeCurve;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _screenCrackOffsetCurve;
    [SerializeField, BoxGroup("Settings"), ColorUsage(true, true)] private Color _backMatStartColor;
    [SerializeField, BoxGroup("Settings"), ColorUsage(true, true)] private Color _backMatEndColor;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _innerEdgeCurve;

    [HideInInspector] public UnityEvent OnFissureOpen;

    private Tween _fissureOpenTween;
    private Tween _screenCrackTween;
    private Tween _backMatColorTween;
    private Tween _delayTween1;
    private Tween _delayTween2;
    private Tween _closureTween;

    void OnEnable()
    {
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
    }

    void OnDisable()
    {
        _fissureOpenTween.Stop();
        _screenCrackTween.Stop();
        _backMatColorTween.Stop();
        _delayTween1.Stop();
        _delayTween2.Stop();
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
    }

    [Button]
    public void Play()
    {
        _backRenderer.material.SetFloat("_InnerEdge", 0);
        _fissureBack.Stop();
        _fissureFront.Stop();
        _fissureCrack.Stop();
        _distortion.Stop();
        _fissureFront.Play();
        _fissureBack.Play();
        _fissureCrack.Play();
        _distortion.Play();

        _fissureOpenTween.Stop();
        _screenCrackTween.Stop();
        _backMatColorTween.Stop();
        _delayTween1.Stop();
        _delayTween2.Stop();

        _fissureOpenTween = Tween.Custom(0, 1, duration: _fissureOpenTime, onValueChange: prog =>
        {
            _frontRenderer.material.SetFloat("_FissureProgress", _fissureProgressCurve.Evaluate(prog));
            _crackRenderer.material.SetFloat("_FissureProgress", _crackProgressCurve.Evaluate(prog));
        }).OnComplete(() =>
        {
            OnFissureOpen.Invoke();
        });

        _delayTween1 = Tween.Delay(_fissureOpenTime / 3).OnComplete(() => _fracture.Play());

        _delayTween2 = Tween.Delay(_fissureOpenTime / 2).OnComplete(() =>
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
    
    [Button]
    public void Close()
    {
        _closureTween.Stop();
        _closureTween = Tween.Custom(0, 1, duration: _fissureCloseTime, onValueChange: prog => _backRenderer.material.SetFloat("_InnerEdge", _innerEdgeCurve.Evaluate(prog)));
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        Close();
    }
}
