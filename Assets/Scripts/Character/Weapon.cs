using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using UnityUtils;
using MoreMountains.Tools;

public class Weapon : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Controller _controller;

    [field: SerializeField, BoxGroup("Data")] private SkillAnimationDatabaseSO _animationDatabase;
    [SerializeField, BoxGroup("Data")] public SkillsSO SkillDatabase;
    [SerializeField, BoxGroup("Data")] public AvailableSkillSO WeaponSkillSO;
    [field: SerializeField, BoxGroup("Skills")] public Dictionary<string, Skill> SkillDict = new Dictionary<string, Skill>();
    [field: SerializeField, BoxGroup("Skills")] public Dictionary<string, PlayableSkill> AvailableSkills { get; private set; } = new Dictionary<string, PlayableSkill>();
    [field: SerializeField, BoxGroup("Skills")] public Dictionary<int, List<string>> WeaponSkillDict = new Dictionary<int, List<string>>();

    private Skill _skill;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private int _abilityIndex = 0;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public List<(string, UpgradeSlotType, bool)> UniversalAttackModifiers = new List<(string, UpgradeSlotType, bool)>();

#if UNITY_EDITOR
    [ShowInInspector] List<string> _availableSkillIds => AvailableSkills.Keys.ToList();
    [ShowInInspector] List<PlayableSkill> _availableSkill => AvailableSkills.Values.ToList();
    [ShowInInspector] List<List<string>> _weaponSkill => WeaponSkillDict.Values.ToList();
#endif


    void OnValidate()
    {
        if (_controller == null) _controller = GetComponentInParent<Controller>();
    }


    void Awake()
    {

        AvailableSkills.OrderBy(kvp => kvp.Value.BaseWeight);
    }

    void OnEnable()
    {
        RefreshSkillDatabase();
        RefreshAvailableWeaponSkills();
        AvailableSkills.OrderBy(kvp => kvp.Value.BaseWeight);
    }

    public void RefreshSkillDatabase()
    {
        if (SkillDatabase.SkillDict.Count > SkillDict.Count)
        {
            SkillDict = new Dictionary<string, Skill>(SkillDatabase.SkillDict).CloneToRuntime(v => new Skill(v));
        }
    }

    public void RefreshAvailableSkills()
    {
        foreach (List<string> skilltype in WeaponSkillDict.Values)
        {
            foreach (string key in skilltype)
            {
                if (!AvailableSkills.ContainsKey(key))
                    AvailableSkills.Add(key, new PlayableSkill(key));
            }
        }
    }

    public void RefreshAvailableWeaponSkills()
    {
        foreach (var key in WeaponSkillSO.SkillDict.Keys)
        {
            List<string> list = WeaponSkillSO.SkillDict[key].Clone();
            if (!WeaponSkillDict.ContainsKey(key))
                WeaponSkillDict.Add(key, list);
            else
                WeaponSkillDict[key] = list;
        }

        RefreshAvailableSkills();
    }

    [Button]
    private void SetupAnimEvents()
    {

    }

    public Skill LoopBasicAttack()
    {
        if (_animationDatabase == null) return null;
        if (SkillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;

        foreach (string key in WeaponSkillDict[0])
        {
            if (!AvailableSkills[key].IsInCooldown)
                if (SkillDict.ContainsKey(AvailableSkills[key].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(SkillDict[AvailableSkills[key].SkillId].AnimationID))
                {
                    StartCoroutine(SkillCooldownCO(key, SkillDict[AvailableSkills[key].SkillId].Cooldown));
                    _skill = SkillDict[AvailableSkills[key].SkillId];
                    return _skill;
                }
        }
        return null;
    }

    public Skill GetExecutionSkill()
    {
        if (_animationDatabase == null) return null;
        if (SkillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;

        _skill = SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Execution][0]];
        return _skill;
    }

    public Skill GetSkill(string skillId)
    {
        if (_animationDatabase == null) return null;
        if (SkillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;

        _skill = SkillDict[skillId];

        return _skill;
    }

    public Skill GetParrySkill()
    {
        _skill = SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Parry][0]];
        return _skill;
    }

    public Skill GetDashSkill()
    {
        _skill = SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Dash][0]];
        return _skill;
    }

    public ClipTransition GetAnimationClip(string animationId)
    {
        return _animationDatabase.SkillAnimDict[animationId];
    }

    private IEnumerator SkillCooldownCO(string key, float cooldownTime)
    {
        AvailableSkills[key].IsInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        AvailableSkills[key].IsInCooldown = false;
    }
    public Skill GetAbility()
    {
        if (_animationDatabase == null) return null;
        if (SkillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;

        _skill = SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Ability][_abilityIndex]];
        return _skill;
    }

    public void UpdateAbilityIndex(bool isUpward)
    {
        if (isUpward)
        {
            _abilityIndex++;
            if (_abilityIndex >= WeaponSkillDict[AVAILABLESKILLKEY.Ability].Count)
            {
                _abilityIndex = 0;
            }
        }
        else
        {
            _abilityIndex--;
            if (_abilityIndex < 0)
            {
                _abilityIndex = WeaponSkillDict[AVAILABLESKILLKEY.Ability].Count - 1;
            }
        }
    }

    public int GetCurrentAbilityIndex()
    {
        return _abilityIndex;
    }

    public Skill GetAttackSkillWithCooldown(float cooldown)
    {
        if (cooldown.Approx(1.2f))
        {
            return SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Attack][0]];
        }
        else if (cooldown.Approx(1f))
        {
            return SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Attack][1]];
        }
        else if (cooldown.Approx(.7f))
        {
            return SkillDict[WeaponSkillDict[AVAILABLESKILLKEY.Attack][2]];
        }

        return null;
    }

    public int GetAttackSkillIndexWithCooldown(float cooldown)
    {
        if (cooldown.Approx(1.2f))
        {
            return 0;
        }
        else if (cooldown.Approx(1f))
        {
            return 1;
        }
        else if (cooldown.Approx(.7f))
        {
            return 2;
        }

        return -1;
    }

    public void AddModifierDirect(string skillId, (string, UpgradeSlotType) modifier, bool isBuff)
    {
        if (isBuff)
        {
            switch (modifier.Item2)
            {
                case UpgradeSlotType.Start:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 0f));
                    SkillDict[skillId].IsStartBuffed = true;
                    break;
                case UpgradeSlotType.Mid:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 0.5f));
                    SkillDict[skillId].IsMidBuffed = true;
                    break;
                case UpgradeSlotType.End:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 1f));
                    SkillDict[skillId].IsEndBuffed = true;
                    break;
            }

        }
        else
        {
            switch (modifier.Item2)
            {
                case UpgradeSlotType.Start:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 0));
                    break;
                case UpgradeSlotType.Mid:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 0.5f));
                    break;
                case UpgradeSlotType.End:
                    SkillDict[skillId].Buffs.Add((modifier.Item1, 1f));
                    break;
            }
        }

    }

    public void AddUniversalAttackModifier((string, UpgradeSlotType) modifier, bool isBuff)
    {
        UniversalAttackModifiers.Add((modifier.Item1, modifier.Item2, isBuff));
    }

    public bool IsStartBuffed()
    {
        for (int i = 0; i < UniversalAttackModifiers.Count; i++)
        {
            if (UniversalAttackModifiers[i].Item2 == UpgradeSlotType.Start)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsMidBuffed()
    {
        for (int i = 0; i < UniversalAttackModifiers.Count; i++)
        {
            if (UniversalAttackModifiers[i].Item2 == UpgradeSlotType.Mid)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEndBuffed()
    {
        for (int i = 0; i < UniversalAttackModifiers.Count; i++)
        {
            if (UniversalAttackModifiers[i].Item2 == UpgradeSlotType.End)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetSkills()
    {
        SkillDict.Clear();
        RefreshSkillDatabase();
        RefreshAvailableWeaponSkills();
        AvailableSkills.OrderBy(kvp => kvp.Value.BaseWeight);
        UniversalAttackModifiers.Clear();
    }

}
