using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

public class Weapon : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Controller _controller;

    [field: SerializeField, BoxGroup("Data")] private SkillAnimationDatabaseSO _animationDatabase;
    [SerializeField, BoxGroup("Data")] public SkillsSO SkillDatabase;
    [SerializeField, BoxGroup("Data")] public AvailableSkillSO WeaponSkillSO;
    [field: SerializeField, BoxGroup("Skills")] public Dictionary<string, PlayableSkill> AvailableSkills { get; private set; } = new Dictionary<string, PlayableSkill>();

    private Skill _skill;

#if UNITY_EDITOR
    [ShowInInspector] List<string> _availableSkillIds => AvailableSkills.Keys.ToList();
    [ShowInInspector] List<PlayableSkill> _availableSkill => AvailableSkills.Values.ToList();
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
        AvailableSkills.OrderBy(kvp => kvp.Value.BaseWeight);
        foreach (List<string> skilltype in WeaponSkillSO.SkillDict.Values)
        {
            foreach (string key in skilltype)
            {
                if (!AvailableSkills.ContainsKey(key))
                    AvailableSkills.Add(key, new PlayableSkill(key));
            }
        }
        
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

        foreach (string key in WeaponSkillSO.SkillDict[0])
        {
            if (!AvailableSkills[key].IsInCooldown)
                if (SkillDatabase.SkillDict.ContainsKey(AvailableSkills[key].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(SkillDatabase.SkillDict[AvailableSkills[key].SkillId].AnimationID))
                {
                    StartCoroutine(SkillCooldownCO(key, SkillDatabase.SkillDict[AvailableSkills[key].SkillId].Cooldown));
                    _skill = SkillDatabase.SkillDict[AvailableSkills[key].SkillId];
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

        _skill = SkillDatabase.SkillDict[WeaponSkillSO.SkillDict[3][0]];
        return _skill;
    }

    public Skill GetSkill(string skillId)
    {
        if (_animationDatabase == null) return null;
        if (SkillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;
        
        _skill = SkillDatabase.SkillDict[skillId];

        return _skill;
    }

    public Skill GetRandomParrySkill()
    {
        _skill = SkillDatabase.SkillDict[WeaponSkillSO.SkillDict[1][Random.Range(0, WeaponSkillSO.SkillDict[1].Count)]];
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
    public void UseAbility(string abilityId)
    {

    }
}
