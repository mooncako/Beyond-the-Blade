using System;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SkillPickupUI : MonoBehaviour, MMEventListener<NewSkillEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private Button _swapButton;
    [SerializeField, BoxGroup("References")] private Button _discardButton;
    [SerializeField, BoxGroup("References")] private Image _currentRarity;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _currentDamageText;
    [SerializeField, BoxGroup("References")] private Image _newRarity;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _newDamageText;
    private Tween _alphaTween;
    private Skill _targetSkill;
    private string _skillId;


    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<NewSkillEvent>();
        _swapButton.onClick.AddListener(SwapSkill);
        _discardButton.onClick.AddListener(Discard);
    }

    

    void OnDisable()
    {
        this.MMEventStopListening<NewSkillEvent>();
        _swapButton.onClick.RemoveListener(SwapSkill);
        _discardButton.onClick.RemoveListener(Discard);
        _alphaTween.Stop();
    }

    private void SwapSkill()
    {
        SkillSwapEvent.Trigger(_targetSkill, _skillId);
        NewSkillEvent.Trigger(EventStateType.OnEventEnd, null, null, "");
        ProgressionCanvasCloseEvent.Trigger();
        gameObject.SetActive(false);
    }

    private void Discard()
    {
        ProgressionCanvasCloseEvent.Trigger();
        gameObject.SetActive(false);
    }

    public void OnMMEvent(NewSkillEvent e)
    {
        if (e.Type == EventStateType.OnEventStart)
        {
            _alphaTween.Stop();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _alphaTween = Tween.Alpha(_canvasGroup, 1, duration: .5f);
            _currentDamageText.text = e.Controller.CurrentWeapon.GetAttackSkillWithCooldown(e.TargetSkill.Cooldown).Damage + " Damage";
            _currentRarity.color = RarityUtil.GetRarityColor(e.Controller.CurrentWeapon.GetAttackSkillWithCooldown(e.TargetSkill.Cooldown).Rarity);
            _newDamageText.text = e.TargetSkill.Damage + " Damage";
            _newRarity.color = RarityUtil.GetRarityColor(e.TargetSkill.Rarity);
            _targetSkill = e.TargetSkill;
            _skillId = e.SkillId;
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
