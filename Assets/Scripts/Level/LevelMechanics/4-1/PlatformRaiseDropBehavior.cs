using System;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformRaiseDropBehavior : MonoBehaviour
{
    
    [SerializeField, BoxGroup("Settings")] private float _raiseHeight = 5f;
    [SerializeField, BoxGroup("Settings")] private float _dropHeight = 5f;
    [SerializeField, BoxGroup("Settings")] private float _shakeDuration = 1.5f;
    [SerializeField, BoxGroup("Settings")] private Vector3 _shakeStrength = new Vector3(0.3f, 0.1f, 0.3f);
    [SerializeField, BoxGroup("Settings")] private float _tweenDuration = .4f;

    private Tween _shakeTween;
    private Tween _dropTween;
    private Tween _raiseTween;
    private Tween _resetTween;

    private float _originalHeight;
    

    private void Start()
    {
        _originalHeight = transform.localPosition.y;
    }

    private void OnDisable()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        _resetTween.Stop();
    }
    
    [Button]
    private void DropPlatform()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        _resetTween.Stop();
        
        _shakeTween = Tween.ShakeLocalPosition(transform, _shakeStrength, _shakeDuration).OnComplete(() =>
        {
            _dropTween = Tween.LocalPositionY(transform, transform.localPosition.y - _dropHeight, _tweenDuration, Ease.InExpo);
        });
    }

    [Button]
    private void RaisePlatform()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        _resetTween.Stop();
        
        _shakeTween = Tween.ShakeLocalPosition(transform, _shakeStrength, _shakeDuration).OnComplete(() =>
        {
            _raiseTween = Tween.LocalPositionY(transform, transform.localPosition.y + _raiseHeight, _tweenDuration, Ease.OutExpo);
        });
    }

    [Button]
    private void ResetPlatform()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        _resetTween.Stop();
        
        _resetTween = Tween.LocalPositionY(transform, _originalHeight, _tweenDuration, Ease.Default);

    }
}
