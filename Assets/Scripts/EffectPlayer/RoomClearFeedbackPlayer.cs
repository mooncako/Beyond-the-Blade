using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomClearFeedbackPlayer : MonoBehaviour,
    MMEventListener<RoomClearedEvent>
{
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _timeScaleCurve;

    private Tween _timeScaleTween;

    void OnEnable()
    {
        this.MMEventStartListening<RoomClearedEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<RoomClearedEvent>();
        _timeScaleTween.Stop();
    }

    public void OnMMEvent(RoomClearedEvent e)
    {
        CameraFocusEvent.Trigger(new CameraLensSetting(10));
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    [Button]
    private void TestFeedback()
    {
        CameraFocusEvent.Trigger(new CameraLensSetting(10));
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    
}
