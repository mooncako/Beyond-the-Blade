using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

public class Posture : MonoBehaviour
{
    [Title("Stun Progress", titleAlignment: TitleAlignments.Centered)]
    [ProgressBar(0, "StunThreshold", ColorGetter = "GetStunThresholdColor", Height = 30), HideLabel] public float CurrentStunValue = 0;
    public float StunPercentile => CurrentStunValue / StunThreshold;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _stunned = false;
    [HideInInspector] public float StunThreshold;
    [HideInInspector] public UnityEvent<float> OnStunned; 
    [HideInInspector] public UnityEvent<float> OnStunUpdate;


    public void ApplyStats(float stunThreshold)
    {
        StunThreshold = stunThreshold;
    }

    public void IncreaseStun(float value, float duration)
    {
        if(_stunned) return;

        CurrentStunValue = Mathf.Clamp(CurrentStunValue + value, 0, StunThreshold);
        OnStunUpdate.Invoke(StunPercentile);
        if(CurrentStunValue.Approx(StunThreshold))
        {
            OnStunned.Invoke(duration);
            StopCoroutine(StunCO(duration));
            StartCoroutine(StunCO(duration));
        }
    }

    public void ClearStun()
    {
        CurrentStunValue = 0;
    }

    private IEnumerator StunCO(float duration)
    {
        _stunned = true;
        while(CurrentStunValue > 0)
        {
            yield return new WaitForSeconds(.1f);
            CurrentStunValue = Mathf.Clamp(CurrentStunValue - StunThreshold/(duration/.1f), 0, StunThreshold);
            OnStunUpdate.Invoke(StunPercentile);
        }
        _stunned = false;
    }

#region Editor Code
    private Color GetStunThresholdColor(float value)
    {
        return Color.Lerp(Color.white, Color.red, value/StunThreshold);
    }

    [Button]
    private void DebugIncreaseStun(float value = 1, float duration = 3)
    {
        if(_stunned) return;
        CurrentStunValue = Mathf.Clamp(CurrentStunValue + value, 0, StunThreshold);
        if(CurrentStunValue.Approx(StunThreshold))
        {
            OnStunned.Invoke(duration);
        }
    }

    [Button]
    private void DebugClearStun()
    {
        CurrentStunValue = 0;
    }

    [Button]
    private void DebugStun(float duration = 3)
    {
        CurrentStunValue = StunThreshold;
        OnStunned.Invoke(duration);
    }
#endregion
}
