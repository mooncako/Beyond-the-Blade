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
    [SerializeField, BoxGroup("References")] private ModifierPickup _skillUpgradePickup;
    [SerializeField, BoxGroup("References")] private PoolableSkillDatabaseSO _poolableSkillDatabase;
    [SerializeField, BoxGroup("References")] private AbilityDatabaseSO _abilityDatabase;
    [SerializeField, BoxGroup("References")] private PoolableModifierDatabaseSO _poolableModifierDatabase;
    [SerializeField, BoxGroup("References")] private PlayerController _player;

    private List<PoolableSkill> _normalAttacks = new List<PoolableSkill>();
    private List<PoolableSkill> _rareAttacks = new List<PoolableSkill>();
    private List<PoolableSkill> _epicAttacks = new List<PoolableSkill>();
    private List<PoolableSkill> _legendaryAttacks = new List<PoolableSkill>();

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
            _normalAttacks.Add(new PoolableSkill(_poolableSkillDatabase.NormalSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Attack].Contains(_normalAttacks[i].SkillId))
            {
                _normalAttacks[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.RareSkills.Count; i++)
        {
            _rareAttacks.Add(new PoolableSkill(_poolableSkillDatabase.RareSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Attack].Contains(_rareAttacks[i].SkillId))
            {
                _rareAttacks[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.EpicSkills.Count; i++)
        {
            _epicAttacks.Add(new PoolableSkill(_poolableSkillDatabase.EpicSkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Attack].Contains(_epicAttacks[i].SkillId))
            {
                _epicAttacks[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _poolableSkillDatabase.LegendarySkills.Count; i++)
        {
            _legendaryAttacks.Add(new PoolableSkill(_poolableSkillDatabase.LegendarySkills[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Attack].Contains(_legendaryAttacks[i].SkillId))
            {
                _legendaryAttacks[i].CanAppear = false;
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
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Contains(_normalAbilities[i].SkillId))
            {
                _normalAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.RareAbilities.Count; i++)
        {
            _rareAbilities.Add(new PoolableSkill(_abilityDatabase.RareAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Contains(_rareAbilities[i].SkillId))
            {
                _rareAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.EpicAbilities.Count; i++)
        {
            _epicAbilities.Add(new PoolableSkill(_abilityDatabase.EpicAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Contains(_epicAbilities[i].SkillId))
            {
                _epicAbilities[i].CanAppear = false;
            }
        }

        for (int i = 0; i < _abilityDatabase.LegendaryAbilities.Count; i++)
        {
            _legendaryAbilities.Add(new PoolableSkill(_abilityDatabase.LegendaryAbilities[i]));
            if (_player.CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Contains(_legendaryAbilities[i].SkillId))
            {
                _legendaryAbilities[i].CanAppear = false;
            }
        }

    }

    private void CalculateAndAssignAbilities()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        string abilityOne = GetRandomUpgradeIndex(rarity, ProgressionType.Ability);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityTwo = GetRandomUpgradeIndex(rarity, ProgressionType.Ability);

        index = Random.Range(0f, 1f);
        rarity = DropRateManager.Instance.GetRarity(index);
        string abilityThree = GetRandomUpgradeIndex(rarity, ProgressionType.Ability);

        PoolableAbilityAssignDataEvent.Trigger(abilityOne, abilityTwo, abilityThree);
    }

    private string GetRandomPoolableSkill()
    {
        float index = Random.Range(0f, 1f);
        Rarity rarity = DropRateManager.Instance.GetRarity(index);
        return GetRandomUpgradeIndex(rarity, ProgressionType.Attack);
    }

    private string GetRandomPoolableModifier()
    {
        Rarity rarity = DropRateManager.Instance.GetRarity(Random.Range(0f, 1f));
        return GetRandomUpgradeIndex(rarity, ProgressionType.Modifier);
    }

    private string GetRandomUpgradeIndex(Rarity rarity, ProgressionType progressionType)
    {

        // Possible endless loop if no skills are available
        while (!IsRarityAvailable(rarity, progressionType))
        {
            rarity = DropRateManager.Instance.GetRarity(Random.Range(0f, 1f));
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

            case ProgressionType.Attack:
                switch (rarity)
                {
                    case Rarity.Normal:
                        int id = Random.Range(0, _normalAttacks.Count);
                        string skill = _normalAttacks[id].SkillId;
                        _normalAttacks.RemoveAt(id);
                        return skill;

                    case Rarity.Rare:
                        id = Random.Range(0, _rareAttacks.Count);
                        skill = _rareAttacks[id].SkillId;
                        _rareAttacks.RemoveAt(id);
                        return skill;

                    case Rarity.Epic:
                        id = Random.Range(0, _epicAttacks.Count);
                        skill = _epicAttacks[id].SkillId;
                        _epicAttacks.RemoveAt(id);
                        return skill;

                    case Rarity.Legendary:
                        id = Random.Range(0, _legendaryAttacks.Count);
                        skill = _legendaryAttacks[id].SkillId;
                        _legendaryAttacks.RemoveAt(id);
                        return skill;
                }
                break;

            case ProgressionType.Modifier:
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

    private bool IsRarityAvailable(Rarity rarity, ProgressionType progressionType)
    {
        switch(progressionType)
        {
            case ProgressionType.Ability:
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
                break;
            case ProgressionType.Attack:
                switch (rarity)
                {
                    case Rarity.Normal:
                        return _normalAttacks.Count > 0;
                    case Rarity.Rare:
                        return _rareAttacks.Count > 0;
                    case Rarity.Epic:
                        return _epicAttacks.Count > 0;
                    case Rarity.Legendary:
                        return _legendaryAttacks.Count > 0;
                }
                break;
            case ProgressionType.Modifier:
                switch (rarity)
                {
                    case Rarity.Normal:
                        return _normalModifiers.Count > 0;
                    case Rarity.Rare:
                        return _rareModifiers.Count > 0;
                    case Rarity.Epic:
                        return _epicModifiers.Count > 0;
                    case Rarity.Legendary:
                        return _legendaryModifiers.Count > 0;
                }
                break;

        }
        
        return false;
    }

    public void SpawnPickup(Vector3 pos, LevelRewardType rewardType)
    {
        switch(rewardType)
        {
            case LevelRewardType.Modifier:
                UpdateModifiers();
                ModifierPickup modifier = Instantiate(_skillUpgradePickup, pos, Quaternion.identity);
                modifier.AssignId(GetRandomPoolableModifier());
                break;
            case LevelRewardType.Attack:
                UpdateSkills();
                SkillPickup attack = Instantiate(_skillPickup, pos, Quaternion.identity);
                attack.AssignId(GetRandomPoolableSkill());
                break;
            case LevelRewardType.Ability:
                UpdateAbilities();
                CalculateAndAssignAbilities();
                Instantiate(_abilityPickup, pos, Quaternion.identity);
                break;
        }
    }

    public void SpawnPickup(Vector3 pos, ProgressionType type)
    {
        switch(type)
        {
            case ProgressionType.Attack:
                UpdateSkills();
                SkillPickup pickup = Instantiate(_skillPickup, pos, Quaternion.identity);
                pickup.AssignId(GetRandomPoolableSkill());
                break;
            case ProgressionType.Modifier:
                UpdateModifiers();
                ModifierPickup upgradePickup = Instantiate(_skillUpgradePickup, pos, Quaternion.identity);
                upgradePickup.AssignId(GetRandomPoolableModifier());
                break;
            case ProgressionType.Ability:
                UpdateAbilities();
                CalculateAndAssignAbilities();
                Instantiate(_abilityPickup, pos, Quaternion.identity);
                break;
        }
    }

    [Button]
    private void DebugSpawnAbility()
    {
        UpdateAbilities();
        CalculateAndAssignAbilities();
        Instantiate(_abilityPickup, Vector3.zero, Quaternion.identity);
    }

    public void OnMMEvent(PlayerInitializedEvent e)
    {
        _player = e.Player;
        UpdateSkills();
        UpdateModifiers();
        UpdateAbilities();
    }


}
