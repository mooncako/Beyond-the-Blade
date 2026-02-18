using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class DisableUiDuringTransition : MonoBehaviour,
    MMEventListener<LevelTransitionEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<LevelTransitionEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<LevelTransitionEvent>();
    }

    public void OnMMEvent(LevelTransitionEvent e)
    {
        if (e.Type == EventStateType.OnEventStarted)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        else
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
    }
}
