using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

public class CameraRotator : MonoBehaviour,
    MMEventListener<CameraRotateEvent>
{
    [SerializeField, BoxGroup("Settings")] private float _startDuration = .1f;
    [SerializeField, BoxGroup("Settings")] private float _recoverDuration = .3f;
    [SerializeField, BoxGroup("Settings")] private float _scale = 2f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _defaultRotation;
    
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector2 _rotation;
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
    private void TriggerRotate(Vector2 input)
    {
        if(input == Vector2.zero)
        {
            _rotateTween.Stop();
            _rotateTween = Tween.LocalEulerAngles(transform, transform.localEulerAngles, _defaultRotation, _recoverDuration);
        }
        else
        {
            // if(input.x > 0 && input.y.Approx(0))
            // {
            //     _rotation.x = 1;
            //     _rotation.y = 0;
            // }else if(input.x < 0 && input.y.Approx(0))
            // {
            //     _rotation.x = -1;
            //     _rotation.y = 0;
            // }else if(input.x > 0 && input.y > 0)
            // {
            //     _rotation.x = 1;
            //     _rotation.y = 1;
            // }else if(input.x > 0 && input.y < 0)
            // {
            //     _rotation.x = 1;
            //     _rotation.y = -1;
            // }else if(input.x < 0 && input.y > 0)
            // {
            //     _rotation.x = -1;
            //     _rotation.y = 1;
            // }

            _rotation = input * -_scale;
            _rotateTween.Stop();

            _rotateTween = Tween.LocalEulerAngles(transform, transform.localEulerAngles, new Vector3(_defaultRotation.x + _rotation.y, _defaultRotation.y + _rotation.x, _defaultRotation.z), _startDuration);
        }
        
    }

    public void OnMMEvent(CameraRotateEvent e)
    {
        TriggerRotate(e.Input);
    }
}
