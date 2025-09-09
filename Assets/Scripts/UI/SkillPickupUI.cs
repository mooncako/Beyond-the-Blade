using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SkillPickupUI : MonoBehaviour, MMEventListener<SkillPickupInteractEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    private Tween _alphaTween;


    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SkillPickupInteractEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SkillPickupInteractEvent>();
        _alphaTween.Stop();
    }

    public void OnMMEvent(SkillPickupInteractEvent e)
    {
        if (e.Type == EventStateType.OnEventStart)
        {
            _alphaTween.Stop();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _alphaTween = Tween.Alpha(_canvasGroup, 1, duration: .5f);
        }
        else
        {
            _alphaTween.Stop();
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _alphaTween = Tween.Alpha(_canvasGroup, 0, duration: .5f);
        }
    }
}
