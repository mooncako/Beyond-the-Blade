using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilityUI : MonoBehaviour,
    MMEventListener<PlayerInitializedEvent>,
    MMEventListener<NewAbilityCallbackEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private PlayerController _player;
    [SerializeField, BoxGroup("References")] private AbilityUIIcon[] _abilityIcons;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isInitialized = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _currentAbilitySelection = 0;
    private Tween _alphaTween;


    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _abilityIcons = GetComponentsInChildren<AbilityUIIcon>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<PlayerInitializedEvent>();
        this.MMEventStartListening<NewAbilityCallbackEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerInitializedEvent>();
        this.MMEventStopListening<NewAbilityCallbackEvent>();
        if (_isInitialized)
        {
            _player.OnAbilityCycled.RemoveListener(CycleAbility);
            _player.OnAbilityStartCooldown.RemoveListener(EnterCooldown);
        }

    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        _isInitialized = true;
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        UpdateIcons();
        _player.OnAbilityCycled.AddListener(CycleAbility);
        CycleAbility();

        _player.OnAbilityStartCooldown.AddListener(EnterCooldown);
    }

    public void OnMMEvent(NewAbilityCallbackEvent e)
    {
        if (e.IsSuccessfullyAdded)
        {
            UpdateIcons();
        }
    }

    private void CycleAbility()
    {
        _abilityIcons[_currentAbilitySelection].Deselect();
        _currentAbilitySelection = _player.CurrentWeapon.GetCurrentAbilityIndex();
        _abilityIcons[_currentAbilitySelection].Select();
    }

    [Button]
    private void UpdateIcons()
    {
        for (int i = 0; i < _player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Count; i++)
        {
            _abilityIcons[i].AssignIcon(_player.CurrentWeapon.SkillDict[_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability][i]].Icon);
        }
    }

    private void EnterCooldown(float cooldown)
    {
        _abilityIcons[_currentAbilitySelection].OnAbilityCooldownStarted(cooldown);
    }

}
