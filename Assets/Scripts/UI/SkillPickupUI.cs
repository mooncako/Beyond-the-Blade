using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SkillPickupUI : MonoBehaviour, MMEventListener<SkillPickupInteractEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private Button _swapButton;
    [SerializeField, BoxGroup("References")] private Image _currentRarity;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _currentDamageText;
    [SerializeField, BoxGroup("References")] private Image _newRarity;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _newDamageText;
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
            _currentDamageText.text = e.Controller.CurrentWeapon.GetPlayerAttackSkill(e.TargetSkill.Cooldown).Damage + " Damage";
            _newDamageText.text = e.TargetSkill.Damage + " Damage";
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
