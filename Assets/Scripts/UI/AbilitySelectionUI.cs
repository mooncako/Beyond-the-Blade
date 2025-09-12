using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilitySelectionUI : MonoBehaviour, MMEventListener<NewAbilityEvent>, MMEventListener<ProgressionCanvasCloseEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private AbilityChoice[] _abilityChoices;
    private Tween _alphaTween;




    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        _abilityChoices = GetComponentsInChildren<AbilityChoice>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<NewAbilityEvent>();
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<NewAbilityEvent>();
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void OnMMEvent(NewAbilityEvent e)
    {
        if (e.Type == EventStateType.OnEventStart)
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        else
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}
