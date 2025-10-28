using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomClearFeedbackPlayer : MonoBehaviour,
    MMEventListener<RoomClearedEvent>
{
    [SerializeField, BoxGroup("References")] private FissurePlayer _fissurePlayerPrefab;

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
        CameraFocusEvent.Trigger(new CameraLensSetting(18));
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true).OnComplete(() =>
        {
            FissurePlayer fissurePlayer = Instantiate(_fissurePlayerPrefab, LevelManager.Instance.GetCurrentPickupSpawnPos(new Vector3(0, 1.5f, 0)), Quaternion.identity);
            fissurePlayer.OnFissureOpen.AddListener(() =>
            {
                fissurePlayer.OnFissureOpen.RemoveAllListeners();
                SpawnRewardEvent.Trigger();
            });
            fissurePlayer.Play();
        });
    }

    [Button]
    private void TestFeedback()
    {
        CameraFocusEvent.Trigger(new CameraLensSetting(18));
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    
}
