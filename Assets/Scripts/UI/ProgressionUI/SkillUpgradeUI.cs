using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeUI : MonoBehaviour, MMEventListener<SkillUpgradeEvent>, MMEventListener<ProgressionCanvasCloseEvent>
{
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _attackSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _executionSlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _parrySlot;
    [SerializeField, BoxGroup("References")] private SkillUpgradeSlotUI _dashSlot;
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierNameText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierDescText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _modifierSlotText;
    [SerializeField, BoxGroup("References")] private Image _modifierRarity;
    [SerializeField, BoxGroup("References")] private Image _modifierIcon;
    [SerializeField, BoxGroup("References")] private ModifierDatabaseSO _skillModifierDatabase;
    [SerializeField, BoxGroup("References")] private Button _discardButton;

    private Tween _alphaTween;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SkillUpgradeEvent>();
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
        _discardButton.onClick.AddListener(Discard);
    }

    void OnDisable()
    {
        this.MMEventStopListening<SkillUpgradeEvent>();
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
        _discardButton.onClick.RemoveListener(Discard);
        _alphaTween.Stop();
    }

    public void OnMMEvent(SkillUpgradeEvent e)
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
            _modifierIcon.sprite = _skillModifierDatabase.SkillModifierDict[e.Modifier.Item1].Icon;
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
    public void OnMMEvent(ProgressionCanvasCloseEvent e)
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
        _attackSlot.AssignName("Attack", "");
        _parrySlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Parry][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Parry][0]);
        _dashSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Dash][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Dash][0]);
        _executionSlot.AssignName(weapon.GetSkill(weapon.WeaponSkillDict[AVAILABLESKILLKEY.Execution][0]).Name, weapon.WeaponSkillDict[AVAILABLESKILLKEY.Execution][0]);

        _attackSlot.AssignModifier(weapon, modifier);
        _parrySlot.AssignModifier(weapon, modifier);
        _dashSlot.AssignModifier(weapon, modifier);
        _executionSlot.AssignModifier(weapon, modifier);
    }

    private void Discard()
    {
        ProgressionCanvasCloseEvent.Trigger();
        gameObject.SetActive(false);
    }
}
