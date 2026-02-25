using System;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDrop : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Transform _baseDropPlatform;
    [SerializeField, BoxGroup("References")] private Transform _baseRaisePlatform;

    [SerializeField, BoxGroup("Settings")] private float _shakeDuration = 1.5f;
    [SerializeField, BoxGroup("Settings")] private Vector3 _shakeStrength = new Vector3(0.3f, 0.1f, 0.3f);
    [SerializeField, BoxGroup("Settings")] private float _dropDuration = .4f;
    [SerializeField, BoxGroup("Settings")] private float _raiseDuration = .4f;

    private Tween _shakeTween;
    private Tween _dropTween;
    private Tween _raiseTween;

    private void OnDisable()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
    }
    
    [Button]
    private void DropPlatform()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        
        _shakeTween = Tween.ShakeLocalPosition(transform, _shakeStrength, _shakeDuration).OnComplete(() =>
        {
            _dropTween = Tween.LocalPositionY(transform, _baseDropPlatform.localPosition.y, _dropDuration, Ease.InExpo);
        });
    }

    [Button]
    private void RaisePlatform()
    {
        _shakeTween.Stop();
        _dropTween.Stop();
        _raiseTween.Stop();
        
        _shakeTween = Tween.ShakeLocalPosition(transform, _shakeStrength, _shakeDuration).OnComplete(() =>
        {
            _raiseTween = Tween.LocalPositionY(transform, _baseRaisePlatform.localPosition.y, _raiseDuration, Ease.OutExpo);
        });
    }
}
