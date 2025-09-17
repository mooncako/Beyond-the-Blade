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

    private List<PoolableSkill> _normalModifiers = new List<PoolableSkill>();
    private List<PoolableSkill> _rareModifiers = new List<PoolableSkill>();
    private List<PoolableSkill> _epicModifiers = new List<PoolableSkill>();
    private List<PoolableSkill> _legendaryModifiers = new List<PoolableSkill>();

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
            _normalModifiers.Add(new PoolableSkill(_poolableModifierDatabase.NormalModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.RareModifiers.Count; i++)
        {
            _rareModifiers.Add(new PoolableSkill(_poolableModifierDatabase.RareModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.EpicModifiers.Count; i++)
        {
            _epicModifiers.Add(new PoolableSkill(_poolableModifierDatabase.EpicModifiers[i]));
        }

        for (int i = 0; i < _poolableModifierDatabase.LegendaryModifiers.Count; i++)
        {
            _legendaryModifiers.Add(new PoolableSkill(_poolableModifierDatabase.LegendaryModifiers[i]));
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

    private void CalculateAndAssignAbilities()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        string abilityOne = GetRandomAbilityIndex(rarity, ProgressionType.Ability);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityTwo = GetRandomAbilityIndex(rarity, ProgressionType.Ability);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityThree = GetRandomAbilityIndex(rarity, ProgressionType.Ability);

        PoolableAbilityAssignDataEvent.Trigger(abilityOne, abilityTwo, abilityThree);
    }

    private string GetRandomPoolableSkill()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        return GetRandomAbilityIndex(rarity, ProgressionType.Skill);
    }

    private string GetRandomPoolableModifier()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        return GetRandomAbilityIndex(rarity, ProgressionType.Upgrade);
    }

    private string GetRandomAbilityIndex(Rarity rarity, ProgressionType progressionType)
    {

        while (!IsRarityAvailable(rarity))
        {
            float index = Random.Range(0f, 1f);
            rarity = DropRateManager.Instance.GetRarity(index);
        }

        switch (progressionType)
        {

            case ProgressionType.Ability:
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
                break;

            case ProgressionType.Skill:
                switch (rarity)
                {
                    case Rarity.Normal:
                        int id = Random.Range(0, _normalSkills.Count);
                        string skill = _normalSkills[id].SkillId;
                        _normalSkills.RemoveAt(id);
                        return skill;

                    case Rarity.Rare:
                        id = Random.Range(0, _rareSkills.Count);
                        skill = _rareSkills[id].SkillId;
                        _rareSkills.RemoveAt(id);
                        return skill;

                    case Rarity.Epic:
                        id = Random.Range(0, _epicSkills.Count);
                        skill = _epicSkills[id].SkillId;
                        _epicSkills.RemoveAt(id);
                        return skill;

                    case Rarity.Legendary:
                        id = Random.Range(0, _legendarySkills.Count);
                        skill = _legendarySkills[id].SkillId;
                        _legendarySkills.RemoveAt(id);
                        return skill;
                }
                break;

            case ProgressionType.Upgrade:
                switch (rarity)
                {
                    case Rarity.Normal:
                        int id = Random.Range(0, _normalModifiers.Count);
                        string modifier = _normalModifiers[id].SkillId;
                        _normalModifiers.RemoveAt(id);
                        return modifier;

                    case Rarity.Rare:
                        id = Random.Range(0, _rareModifiers.Count);
                        modifier = _rareModifiers[id].SkillId;
                        _rareModifiers.RemoveAt(id);
                        return modifier;

                    case Rarity.Epic:
                        id = Random.Range(0, _epicModifiers.Count);
                        modifier = _epicModifiers[id].SkillId;
                        _epicModifiers.RemoveAt(id);
                        return modifier;

                    case Rarity.Legendary:
                        id = Random.Range(0, _legendaryModifiers.Count);
                        modifier = _legendaryModifiers[id].SkillId;
                        _legendaryModifiers.RemoveAt(id);
                        return modifier;
                }
                break;
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
        float index = Random.Range(0f, 1f);
        if (index <= DropRateManager.Instance.Skill)
        {
            UpdateSkills();
            SkillPickup pickup = Instantiate(_skillPickup, pos, Quaternion.identity);
            pickup.AssignId(GetRandomPoolableSkill());
        }
        else if (index <= DropRateManager.Instance.Modifier)
        {
            UpdateModifiers();
            SkillUpgradePickup pickup = Instantiate(_skillUpgradePickup, pos, Quaternion.identity);
            pickup.AssignId(GetRandomPoolableModifier());
        }
        else if (index <= DropRateManager.Instance.Ability)
        {
            UpdateAbilities();
            CalculateAndAssignAbilities();
            Instantiate(_abilityPickup, pos, Quaternion.identity);
        }
        else
        {
            NewProgressionDropEvent.Trigger(ProgressionType.Stats);
            Instantiate(_itemPickup, pos, Quaternion.identity);
        }
    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        UpdateSkills();
        UpdateModifiers();
        UpdateAbilities();
    }
}
