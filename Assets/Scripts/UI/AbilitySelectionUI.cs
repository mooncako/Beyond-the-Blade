using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class AbilitySelectionUI : MonoBehaviour,
    MMEventListener<NewAbilityEvent>,
    MMEventListener<ProgressionCanvasCloseEvent>,
    MMEventListener<NewAbilityCallbackEvent>,
    MMEventListener<PlayerInitializedEvent>,
    MMEventListener<NewProgressionDropEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private AbilityChoice[] _abilityChoices;
    [SerializeField, BoxGroup("References")] private AbilitySwapUI[] _abilitySwapUIs;
    [SerializeField, BoxGroup("References")] private GameObject _selectionPanel;
    [SerializeField, BoxGroup("References")] private CanvasGroup _swapPanel;
    [SerializeField, BoxGroup("References")] private AbilityChoice _swapChoice;
    [SerializeField, BoxGroup("References")] private AbilityDatabaseSO _abilityDatabase;
    [SerializeField, BoxGroup("References")] private PlayerController _player;
    [SerializeField, BoxGroup("Debug")] private string _abilityOne;
    [SerializeField, BoxGroup("Debug")] private string _abilityTwo;
    [SerializeField, BoxGroup("Debug")] private string _abilityThree;
    private List<PooledAbility> _normalAbilities = new List<PooledAbility>();
    private List<PooledAbility> _rareAbilities = new List<PooledAbility>();
    private List<PooledAbility> _epicAbilities = new List<PooledAbility>();
    private List<PooledAbility> _legendaryAbilities = new List<PooledAbility>();
    private Tween _alphaTween;
    private string _tempSkillId;


    void OnEnable()
    {
        this.MMEventStartListening<NewAbilityEvent>();
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
        this.MMEventStartListening<NewAbilityCallbackEvent>();
        this.MMEventStartListening<PlayerInitializedEvent>();
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
        this.MMEventStopListening<PlayerInitializedEvent>();
        for (int i = 0; i < _abilitySwapUIs.Length; i++)
        {
            _abilitySwapUIs[i].OnClick.RemoveListener(SwapSkill);
        }
    }

    public void OnMMEvent(ProgressionCanvasCloseEvent e)
    {
        _alphaTween.Stop();
        _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
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
            UpdateAbilities();
        }
        else
        {
            _selectionPanel.SetActive(false);
            _tempSkillId = e.SkillId;
            _swapChoice.AssignData( _player.CurrentWeapon.GetSkill(e.SkillId), e.SkillId);
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_swapPanel, 1, .5f);
            _swapPanel.blocksRaycasts = true;
            _swapPanel.interactable = true;

            for (int i = 0; i < _player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Count; i++)
            {
                _abilitySwapUIs[i].AssignData(_player.CurrentWeapon.SkillDict[_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability][i]], i);
            }
        }


    }

    public void OnMMEvent(NewAbilityEvent e)
    {
        if (e.Type == EventStateType.OnEventStart)
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
    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        UpdateAbilities();
    }

    public void OnMMEvent(NewProgressionDropEvent e)
    {
        if (e.ProgressionType == ProgressionType.Ability)
        {
            CalculateAndAssignAbilities();
        }
    }

    private void UpdateAbilities()
    {
        for (int i = 0; i < _abilityDatabase.NormalAbilities.Count; i++)
        {
            _normalAbilities.Add(new PooledAbility(_abilityDatabase.NormalAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_normalAbilities[i].SkillId))
            {
                _normalAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.RareAbilities.Count; i++)
        {
            _rareAbilities.Add(new PooledAbility(_abilityDatabase.RareAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_rareAbilities[i].SkillId))
            {
                _rareAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.EpicAbilities.Count; i++)
        {
            _epicAbilities.Add(new PooledAbility(_abilityDatabase.EpicAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_epicAbilities[i].SkillId))
            {
                _epicAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.LegendaryAbilities.Count; i++)
        {
            _legendaryAbilities.Add(new PooledAbility(_abilityDatabase.LegendaryAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_legendaryAbilities[i].SkillId))
            {
                _legendaryAbilities[i].CanAppear = false;
            }
        }

    }

    [Button]
    private void CalculateAndAssignAbilities()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        _abilityOne = GetRandomAbilityIndex(rarity);
        _abilityChoices[0].AssignData(_player.CurrentWeapon.SkillDict[_abilityOne], _abilityOne);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        _abilityTwo = GetRandomAbilityIndex(rarity);
        _abilityChoices[1].AssignData(_player.CurrentWeapon.SkillDict[_abilityTwo], _abilityTwo);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        _abilityThree = GetRandomAbilityIndex(rarity);
        _abilityChoices[2].AssignData(_player.CurrentWeapon.SkillDict[_abilityThree], _abilityThree);
    }

    private string GetRandomAbilityIndex(Rarity rarity)
    {

        while (!IsRarityAvailable(rarity))
        {
            float index = Random.Range(0f, 1f);
            rarity = DropRateManager.Instance.GetRarity(index);
        }

        switch (rarity)
        {
            case Rarity.Normal:
                int id = Random.Range(0, _normalAbilities.Count);
                string ability = _normalAbilities[id].SkillId;
                _normalAbilities.RemoveAt(id);
                return ability;

            case Rarity.Rare:
                id = Random.Range(0, _rareAbilities.Count);
                ability = _rareAbilities[id].SkillId;
                _rareAbilities.RemoveAt(id);
                return ability;

            case Rarity.Epic:
                id = Random.Range(0, _epicAbilities.Count);
                ability = _epicAbilities[id].SkillId;
                _epicAbilities.RemoveAt(id);
                return ability;

            case Rarity.Legendary:
                id = Random.Range(0, _legendaryAbilities.Count);
                ability = _legendaryAbilities[id].SkillId;
                _legendaryAbilities.RemoveAt(id);
                return ability;

        }
        return "";
    }

    private bool IsRarityAvailable(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Normal:
                return _normalAbilities.Count > 0;
            case Rarity.Rare:
                return _rareAbilities.Count > 0;
            case Rarity.Epic:
                return _epicAbilities.Count > 0;
            case Rarity.Legendary:
                return _legendaryAbilities.Count > 0;
        }
        return false;
    }

    private void SwapSkill(int index)
    {
        AbilitySwapEvent.Trigger(_tempSkillId, index);

    }
}
