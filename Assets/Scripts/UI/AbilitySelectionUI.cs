using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class AbilitySelectionUI : MonoBehaviour, MMEventListener<NewAbilityEvent>, MMEventListener<ProgressionCanvasCloseEvent>, MMEventListener<NewAbilityCallbackEvent>, MMEventListener<PlayerInitializedEvent>
{
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private AbilityChoice[] _abilityChoices;
    [SerializeField, BoxGroup("References")] private GameObject _selectionPanel;
    [SerializeField, BoxGroup("References")] private GameObject _swapPanel;
    [SerializeField, BoxGroup("References")] private AbilityChoice _swapChoice;
    [SerializeField, BoxGroup("References")] private AbilityDatabaseSO _abilityDatabase;
    [SerializeField, BoxGroup("References")] private PlayerController _player;
    [SerializeField, BoxGroup("Debug")] private string _abilityOne;
    [SerializeField, BoxGroup("Debug")] private string _abilityTwo;
    [SerializeField, BoxGroup("Debug")] private string _abilityThree;
    private List<PooledAbility> _abilities;
    private Tween _alphaTween;




    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<NewAbilityEvent>();
        this.MMEventStartListening<ProgressionCanvasCloseEvent>();
        this.MMEventStartListening<NewAbilityCallbackEvent>();
        this.MMEventStartListening<PlayerInitializedEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<NewAbilityEvent>();
        this.MMEventStopListening<ProgressionCanvasCloseEvent>();
        this.MMEventStopListening<NewAbilityCallbackEvent>();
        this.MMEventStopListening<PlayerInitializedEvent>();
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
            _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        else
        {
            _selectionPanel.SetActive(false);
            _swapChoice.AssignData(_player.CurrentWeapon.GetSkill(e.SkillId));
            _swapPanel.SetActive(true);
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

    private void UpdateAbilities()
    {
        for (int i = 0; i < _abilityDatabase.Abilities.Count; i++)
        {
            _abilities.Add(new PooledAbility(_abilityDatabase.Abilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_abilities[i].SkillId))
            {
                _abilities[i].CanAppear = false;
            }
        }

        AbilityDatabaseUtil.UpdateProbabilities(_abilities, _player.CurrentWeapon.SkillDict);

    }

    private void AssignAbilities()
    {
        //TODO: Calculate the spawned abilities and assign it to the corresponding section 
    }

    
}
