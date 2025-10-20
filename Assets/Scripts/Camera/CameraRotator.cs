using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraRotator : MonoBehaviour,
    MMEventListener<CameraRotateEvent>
{
    [SerializeField, BoxGroup("Settings")] private float _startDuration = 1f;
    [SerializeField, BoxGroup("Settings")] private float _recoverDuration = .3f;
    [SerializeField, BoxGroup("Settings")] private float _xOffsetRange = 5f;
    [SerializeField, BoxGroup("Settings")] private float _yOffsetRange = 5f;
    [SerializeField, BoxGroup("Settings")] private float _zOffsetRange = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _defaultRotation;
    

    private Tween _rotateTween;

    void OnValidate()
    {
        _defaultRotation = transform.localEulerAngles;
    }

    void OnEnable()
    {
        this.MMEventStartListening<CameraRotateEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<CameraRotateEvent>();
        _rotateTween.Stop();
    }

    [Button]
    private void TriggerRotate()
    {
        _rotateTween.Stop();
        _rotateTween = Tween.LocalEulerAngles(transform, transform.localEulerAngles, new Vector3(_defaultRotation.x + Random.Range(-_xOffsetRange, _xOffsetRange), _defaultRotation.y + Random.Range(-_yOffsetRange, _yOffsetRange), _defaultRotation.z + Random.Range(-_zOffsetRange, _zOffsetRange)), _startDuration).OnComplete(() =>
        {
            _rotateTween = Tween.LocalEulerAngles(transform, transform.localEulerAngles, _defaultRotation, _recoverDuration);
        });
    }

    public void OnMMEvent(CameraRotateEvent e)
    {
        TriggerRotate();
    }
}
