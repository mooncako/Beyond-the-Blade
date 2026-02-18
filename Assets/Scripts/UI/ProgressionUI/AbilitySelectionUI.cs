using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionUI : MonoBehaviour,
    MMEventListener<PlayerInitializedEvent>,
    MMEventListener<NewAbilityEvent>,
    MMEventListener<ProgressionCanvasCloseEvent>,
    MMEventListener<NewAbilityCallbackEvent>,
    MMEventListener<PoolableAbilityAssignDataEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private AbilityChoice[] _abilityChoices = new AbilityChoice[3];
    [SerializeField, BoxGroup("References")] private AbilitySwapUI[] _abilitySwapUIs;
    [SerializeField, BoxGroup("References")] private GameObject _selectionPanel;
    [SerializeField, BoxGroup("References")] private CanvasGroup _swapPanel;
    [SerializeField, BoxGroup("References")] private AbilityChoice _swapChoice;
    [SerializeField, BoxGroup("References")] private AbilityDatabaseSO _abilityDatabase;
    [SerializeField, BoxGroup("References")] private PlayerController _player;
    [SerializeField, BoxGroup("References")] private Button _discardButton;
    [SerializeField, BoxGroup("Debug")] private string _abilityOne;
    [SerializeField, BoxGroup("Debug")] private string _abilityTwo;
    [SerializeField, BoxGroup("Debug")] private string _abilityThree;
    
    private Tween _alphaTween;
    private string _tempSkillId;


    void OnEnable()
    {
        this.MMEventStartListening<NewAbilityEvent>();
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
        this.MMEventStartListening<NewAbilityCallbackEvent>();
        this.MMEventStartListening<PoolableAbilityAssignDataEvent>();
        this.MMEventStartListening<PlayerInitializedEvent>();
        _discardButton.onClick.AddListener(Discard);
        for (int i = 0; i < _abilitySwapUIs.Length; i++)
        {
            _abilitySwapUIs[i].OnClick.AddListener(SwapSkill);
        }
    }

    void OnDisable()
    {
        this.MMEventStopListening<NewAbilityEvent>();
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
        this.MMEventStopListening<NewAbilityCallbackEvent>();
        this.MMEventStopListening<PoolableAbilityAssignDataEvent>();
        this.MMEventStopListening<PlayerInitializedEvent>();
        _discardButton.onClick.RemoveListener(Discard);
        for (int i = 0; i < _abilitySwapUIs.Length; i++)
        {
            _abilitySwapUIs[i].OnClick.RemoveListener(SwapSkill);
        }
    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private void Discard()
    {
        ProgressionCanvasCloseEvent.Trigger();
        gameObject.SetActive(false);
    }

    public void OnMMEvent(NewAbilityCallbackEvent e)
    {
        if (e.IsSuccessfullyAdded)
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f).OnComplete(() =>
            {
                _swapPanel.alpha = 0;
                _swapPanel.blocksRaycasts = false;
                _swapPanel.interactable = false;
                _selectionPanel.SetActive(true);
            });
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            // UpdateAbilities();
            ProgressionCanvasCloseEvent.Trigger();
        }
        else
        {
            _selectionPanel.SetActive(false);
            _tempSkillId = e.SkillId;
            _swapChoice.AssignData(_player.CurrentWeapon.GetSkill(e.SkillId), e.SkillId);
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_swapPanel, 1, .5f);
            _swapPanel.blocksRaycasts = true;
            _swapPanel.interactable = true;

            for (int i = 0; i < _player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Count; i++)
            {
                _abilitySwapUIs[i].AssignData(_player.CurrentWeapon.SkillDict[_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability][i]], i);
            }
        }


    }

    public void OnMMEvent(NewAbilityEvent e)
    {
        if (e.Type == EventStateType.OnEventStarted)
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
    // public void OnMMEvent(PlayerInitializedEvent e)
    // {
    //     _player = e.Player;
    //     UpdateAbilities();
    // }

    // public void OnMMEvent(NewProgressionDropEvent e)
    // {
    //     if (e.ProgressionType == ProgressionType.Ability)
    //     {
    //         CalculateAndAssignAbilities();
    //     }
    // }


    public void OnMMEvent(PoolableAbilityAssignDataEvent e)
    {
        _abilityOne = e.AbilityOne;
        _abilityTwo = e.AbilityTwo;
        _abilityThree = e.AbilityThree;
        _abilityChoices[0].AssignData(_player.CurrentWeapon.SkillDict[_abilityOne], _abilityOne);
        _abilityChoices[1].AssignData(_player.CurrentWeapon.SkillDict[_abilityTwo], _abilityTwo);
        _abilityChoices[2].AssignData(_player.CurrentWeapon.SkillDict[_abilityThree], _abilityThree);
    }


    private void SwapSkill(int index)
    {
        AbilitySwapEvent.Trigger(_tempSkillId, index);
        
    }

    
}
