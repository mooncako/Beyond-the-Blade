using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils;

public class CustomTimeScale : MonoBehaviour,
    MMEventListener<WitchTimeEvent>
{
    [SerializeField, BoxGroup("Settings")] private TeamType _team;
    [SerializeField, BoxGroup("Settings")] private float _baseTimeScale = 1;
    [ProgressBar(0, 2, ColorGetter = "GetTimeScaleColor", Height = 30)] public float CurrentTimeScale = 1;

    private float _currentChangedTimeScale = -1;
    private Coroutine _activeTimeScaleCoroutine;

    private void Start()
    {
        CurrentTimeScale = _baseTimeScale;
    }

    private void OnEnable()
    {
        this.MMEventStartListening<WitchTimeEvent>();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening<WitchTimeEvent>();
    }
    
    public void ApplyTimeScale(float value, float duration)
    {
        if (_currentChangedTimeScale.Approx(-1))
        {
            _currentChangedTimeScale = value;
            _activeTimeScaleCoroutine = StartCoroutine(AdjustTimeScaleCO(value, duration));
        }
        else
        {
            if (value < 1 && _currentChangedTimeScale < 1)
            {
                if(_currentChangedTimeScale >= value)
                {
                    _currentChangedTimeScale = value;
                    StopCoroutine(_activeTimeScaleCoroutine);
                    _activeTimeScaleCoroutine = StartCoroutine(AdjustTimeScaleCO(value, duration)) ;
                }
            }else if (value > 1)
            {
                if (_currentChangedTimeScale <= value)
                {
                    _currentChangedTimeScale = value;
                    StopCoroutine(_activeTimeScaleCoroutine);
                    _activeTimeScaleCoroutine = StartCoroutine(AdjustTimeScaleCO(value, duration)) ;
                }
            }
        }
    }

    private IEnumerator AdjustTimeScaleCO(float value, float duration)
    {
        CurrentTimeScale = value;
        yield return new WaitForSeconds(duration);
        CurrentTimeScale = _baseTimeScale;
    }
    
    public void OnMMEvent(WitchTimeEvent e)
    {
        if(e.TargetTeam != _team) return;
        ApplyTimeScale(e.TimeScale, e.Duration);
    }
    
#region editor code    
    private Color GetTimeScaleColor(float value)
    {
        return Color.Lerp(Color.blue, Color.green, value/2);
    }
#endregion


}
