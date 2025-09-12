using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class AbilityUI : MonoBehaviour, MMEventListener<PlayerInitializedEvent>
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
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerInitializedEvent>();
        if (_isInitialized)
        {
            _player.OnAbilityCycled.RemoveListener(CycleAbility);
        }
    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        _isInitialized = true;
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
        _player.OnAbilityCycled.AddListener(CycleAbility);
        CycleAbility();
    }

    private void CycleAbility()
    {
        _abilityIcons[_currentAbilitySelection].Deselect();
        _currentAbilitySelection = _player.CurrentWeapon.GetCurrentAbilityIndex();
        _abilityIcons[_currentAbilitySelection].Select();
    }

}
