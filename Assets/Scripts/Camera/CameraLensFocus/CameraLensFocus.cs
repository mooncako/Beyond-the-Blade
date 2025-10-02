using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class CameraLensFocus : MonoBehaviour,
    MMEventListener<CameraFocusEvent>
{
    [SerializeField, FoldoutGroup("References")] private CinemachineCamera _cineCam;
    [SerializeField, BoxGroup("Debug")] private float _defaultFov;

    private Tween _fovTween;

    void OnValidate()
    {
        if (_cineCam == null)
        {
            _cineCam = GetComponent<CinemachineCamera>();
            _defaultFov = _cineCam.Lens.FieldOfView;
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<CameraFocusEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<CameraFocusEvent>();
        _fovTween.Stop();
    }

    public void OnMMEvent(CameraFocusEvent e)
    {
        _fovTween.Stop();
        _fovTween = Tween.Custom(_defaultFov, e.Setting.Fov, duration: 1f, onValueChange: fov => _cineCam.Lens.FieldOfView = fov, useUnscaledTime: true).OnComplete(() =>
        {
            _fovTween = Tween.Custom(_cineCam.Lens.FieldOfView, _defaultFov, duration: .4f, onValueChange: fov => _cineCam.Lens.FieldOfView = fov, useUnscaledTime: true);
        });

    }
}
