using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class PickupFactory : MonoBehaviour,
    MMEventListener<PlayerInitializedEvent>
{
    [SerializeField, BoxGroup("References")] private GameObject _itemPickup;
    
    [SerializeField, BoxGroup("References")] private GameObject _abilityPickup;
    [SerializeField, BoxGroup("References")] private SkillPickup _skillPickup;
    [SerializeField, BoxGroup("References")] private SkillUpgradePickup _skillUpgradePickup;
    [SerializeField, BoxGroup("References")] private PoolableSkillDatabaseSO _poolableSkillDatabase;
    [SerializeField, BoxGroup(("References"))] private AbilityDatabaseSO _abilityDatabase;
    [SerializeField, BoxGroup("References")] private PoolableModifierDatabaseSO _poolableModifierDatabase;
    [SerializeField, BoxGroup("References")] private PlayerController _player;

    private List<PoolableSkill> _normalSkills = new List<PoolableSkill>();
    private List<PoolableSkill> _rareSkills = new List<PoolableSkill>();
    private List<PoolableSkill> _epicSkills = new List<PoolableSkill>();
    private List<PoolableSkill> _legendarySkills = new List<PoolableSkill>();

    private List<PoolableSkill> _normalAbilities = new List<PoolableSkill>();
    private List<PoolableSkill> _rareAbilities = new List<PoolableSkill>();
    private List<PoolableSkill> _epicAbilities = new List<PoolableSkill>();
    private List<PoolableSkill> _legendaryAbilities = new List<PoolableSkill>();

    void OnEnable()
    {
        this.MMEventStartListening<PlayerInitializedEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerInitializedEvent>();
    }

    private void UpdateSkills()
    {
        for (int i = 0; i < _poolableSkillDatabase.NormalSkills.Count; i++)
        {
            _normalSkills.Add(new PoolableSkill(_poolableSkillDatabase.NormalSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack].Contains(_normalSkills[i].SkillId))
            {
                _normalSkills[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.RareSkills.Count; i++)
        {
            _rareSkills.Add(new PoolableSkill(_poolableSkillDatabase.RareSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack].Contains(_rareSkills[i].SkillId))
            {
                _rareSkills[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.EpicSkills.Count; i++)
        {
            _epicSkills.Add(new PoolableSkill(_poolableSkillDatabase.EpicSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack].Contains(_epicSkills[i].SkillId))
            {
                _epicSkills[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.LegendarySkills.Count; i++)
        {
            _legendarySkills.Add(new PoolableSkill(_poolableSkillDatabase.LegendarySkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Attack].Contains(_legendarySkills[i].SkillId))
            {
                _legendarySkills[i].CanAppear = false;
            }
        }

    }

    private void UpdateModifiers()
    {
        for (int i = 0; i < _poolableModifierDatabase.NormalModifiers.Count; i++)
        {
            _normalSkills.Add(new PoolableSkill(_poolableModifierDatabase.NormalModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.RareModifiers.Count; i++)
        {
            _rareSkills.Add(new PoolableSkill(_poolableModifierDatabase.RareModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.EpicModifiers.Count; i++)
        {
            _epicSkills.Add(new PoolableSkill(_poolableModifierDatabase.EpicModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.LegendaryModifiers.Count; i++)
        {
            _legendarySkills.Add(new PoolableSkill(_poolableModifierDatabase.LegendaryModifiers[i]));
        }

    }

    private void UpdateAbilities()
    {
        for (int i = 0; i < _abilityDatabase.NormalAbilities.Count; i++)
        {
            _normalAbilities.Add(new PoolableSkill(_abilityDatabase.NormalAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_normalAbilities[i].SkillId))
            {
                _normalAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.RareAbilities.Count; i++)
        {
            _rareAbilities.Add(new PoolableSkill(_abilityDatabase.RareAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_rareAbilities[i].SkillId))
            {
                _rareAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.EpicAbilities.Count; i++)
        {
            _epicAbilities.Add(new PoolableSkill(_abilityDatabase.EpicAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AVAILABLESKILLKEY.Ability].Contains(_epicAbilities[i].SkillId))
            {
                _epicAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.LegendaryAbilities.Count; i++)
        {
            _legendaryAbilities.Add(new PoolableSkill(_abilityDatabase.LegendaryAbilities[i]));
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
        string abilityOne = GetRandomAbilityIndex(rarity);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityTwo = GetRandomAbilityIndex(rarity);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityThree = GetRandomAbilityIndex(rarity);

        PoolableAbilityAssignDataEvent.Trigger(abilityOne, abilityTwo, abilityThree);
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

    public void SpawnPickup(Vector3 pos)
    {

    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        UpdateSkills();
        UpdateModifiers();
    }
}
