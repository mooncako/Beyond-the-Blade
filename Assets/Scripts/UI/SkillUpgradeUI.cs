using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeUI : MonoBehaviour, MMEventListener<SkillUpgradePickupInteractEvent>, MMEventListener<PickupUsedEvent>
{
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _attackOneSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _attackTwoSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _attackThreeSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _abilitySlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _executionSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _parrySlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _dashSlot;
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierNameText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierDescText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierSlotText;
    [SerializeField, BoxGroup("References")] private Image _modifierRarity;
    [SerializeField, BoxGroup("References")] private ModifierDatabaseSO _skillModifierDatabase;

    private Tween _alphaTween;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SkillUpgradePickupInteractEvent>();
        this.MMEventStartListening<PickupUsedEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SkillUpgradePickupInteractEvent>();
        this.MMEventStopListening<PickupUsedEvent>();
        _alphaTween.Stop();
    }

    public void OnMMEvent(SkillUpgradePickupInteractEvent e)
    {
        if (e.Type == EventStateType.OnEventStart)
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 1, duration: .5f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            AssignData(e.Weapon, e.Modifier);
            _modifierNameText.text = _skillModifierDatabase.SkillModifierDict[e.Modifier.Item1].Name;
            _modifierDescText.text = _skillModifierDatabase.SkillModifierDict[e.Modifier.Item1].Description;
            _modifierRarity.color = RarityUtil.GetRarityColor(_skillModifierDatabase.SkillModifierDict[e.Modifier.Item1].Rarity);
            switch (e.Modifier.Item2)
            {
                case UpgradeSlotType.Start:
                    _modifierSlotText.text = "I";
                    break;
                case UpgradeSlotType.Mid:
                    _modifierSlotText.text = "II";
                    break;
                case UpgradeSlotType.End:
                    _modifierSlotText.text = "III";
                    break;
            } // Might need to change
        }
        else
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 0, duration: .5f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
    public void OnMMEvent(PickupUsedEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, duration: .5f);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    private void AssignData(Weapon weapon, (string, UpgradeSlotType) modifier)
    {
        _attackOneSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][0]);
        _attackTwoSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][1]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][1]);
        _attackThreeSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][2]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack][2]);
        _abilitySlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability][0]);
        _parrySlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Parry][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Parry][0]);
        _dashSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Dash][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Dash][0]);
        _executionSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Execution][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Execution][0]);

        _attackOneSlot.AssignModifier(weapon, modifier);
        _attackTwoSlot.AssignModifier(weapon, modifier);
        _attackThreeSlot.AssignModifier(weapon, modifier);
        _abilitySlot.AssignModifier(weapon, modifier);
        _parrySlot.AssignModifier(weapon, modifier);
        _dashSlot.AssignModifier(weapon, modifier);
        _executionSlot.AssignModifier(weapon, modifier);
    }

    
}
