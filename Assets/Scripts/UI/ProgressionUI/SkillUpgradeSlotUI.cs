using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUpgradeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _startSlotCanvasGroup;
    [SerializeField, BoxGroup("References")] private CanvasGroup _midSlotCanvasGroup;
    [SerializeField, BoxGroup("References")] private CanvasGroup _endSlotCanvasGroup;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _skillText;

    private string _skillId;
    private (string, UpgradeSlotType) _modifier;
    private Weapon _weapon;
    private Tween _alphaTween;

    void OnValidate()
    {
        if (_startSlotCanvasGroup == null) _startSlotCanvasGroup = GetComponentsInChildren<CanvasGroup>()[0];
        if (_midSlotCanvasGroup == null) _midSlotCanvasGroup = GetComponentsInChildren<CanvasGroup>()[1];
        if (_endSlotCanvasGroup == null) _endSlotCanvasGroup = GetComponentsInChildren<CanvasGroup>()[2];
        if (_skillText == null) _skillText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (_modifier.Item1 != "")
        {
            _weapon.AddModifier(_skillId, _modifier, true);
            ProgressionCanvasCloseEvent.Trigger();
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {

    }

    public void OnPointerExit(PointerEventData e)
    {

    }

    public void AssignModifier(Weapon weapon, (string, UpgradeSlotType) modifier)
    {

        _modifier = ("", UpgradeSlotType.Start);
        switch (modifier.Item2)
        {
            case UpgradeSlotType.Start:
                if (!weapon.GetSkill(_skillId).IsStartBuffed)
                {
                    _modifier = modifier;
                    _startSlotCanvasGroup.alpha = 0;
                }
                else
                {
                    _startSlotCanvasGroup.alpha = 1;
                }
                    
                break;
            case UpgradeSlotType.Mid:
                if (!weapon.GetSkill(_skillId).IsMidBuffed)
                {
                    _modifier = modifier;
                    _startSlotCanvasGroup.alpha = 0;
                }
                else
                {
                    _startSlotCanvasGroup.alpha = 1;
                }
                break;
            case UpgradeSlotType.End:
                if (!weapon.GetSkill(_skillId).IsEndBuffed)
                {
                    _modifier = modifier;
                    _startSlotCanvasGroup.alpha = 0;
                }
                else
                {
                    _startSlotCanvasGroup.alpha = 1;
                }
                break;
        }

        _weapon = weapon;

    }


    public void AssignName(string name, string skillId)
    {
        _skillText.text = name;
        _skillId = skillId;
    }
}
