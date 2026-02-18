using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomClearFeedbackPlayer : MonoBehaviour,
    MMEventListener<LevelClearedEvent>,
    MMEventListener<BeginHitStopEvent>
{
    [SerializeField, BoxGroup("References")] private FissurePlayer _fissurePlayerPrefab;

    [SerializeField, BoxGroup("Settings")] private AnimationCurve _timeScaleCurve;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve _hitStopTimeScaleCurve;

    private Tween _timeScaleTween;
 
    void OnEnable()
    {
        this.MMEventStartListening<LevelClearedEvent>();
        this.MMEventStartListening<BeginHitStopEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<LevelClearedEvent>();
        this.MMEventStopListening<BeginHitStopEvent>();
        _timeScaleTween.Stop();
    }

    public void OnMMEvent(LevelClearedEvent e)
    {
        if(e.TriggerSlowMo)
        {
            CameraFocusEvent.Trigger(new CameraLensSetting(18));
            _timeScaleTween.Stop();
            _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true).OnComplete(() =>
            {
                // FissurePlayer fissurePlayer = Instantiate(_fissurePlayerPrefab, LevelManager.Instance.GetCurrentPickupSpawnPos(new Vector3(0, 1.5f, 0)), Quaternion.identity);
                // fissurePlayer.OnFissureOpen.AddListener(() =>
                // {
                //     fissurePlayer.OnFissureOpen.RemoveAllListeners();
                //     SpawnRewardEvent.Trigger();
                // });
                // fissurePlayer.Play();
                SpawnRewardEvent.Trigger(e.RewardType);
            });
        }
        else
        {
            SpawnRewardEvent.Trigger(e.RewardType);
        }
        
    }

    public void OnMMEvent(BeginHitStopEvent e)
    {
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, .2f, onValueChange: newVal => Time.timeScale = _hitStopTimeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    [Button]
    private void TestFeedback()
    {
        CameraFocusEvent.Trigger(new CameraLensSetting(18));
        _timeScaleTween.Stop();
        _timeScaleTween = Tween.Custom(0, 1, 1, onValueChange: newVal => Time.timeScale = _timeScaleCurve.Evaluate(newVal), cycles: 2, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }

    
}
