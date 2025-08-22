using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

public class Weapon : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Data")] private SkillAnimationDatabaseSO _animationDatabase;
    [SerializeField, BoxGroup("Data")] private SkillsSO _skillDatabase;
    [SerializeField, BoxGroup("Data")] public AvailableSkillSO WeaponSkillSO;
    [field: SerializeField, BoxGroup("Skills")] public Dictionary<string, PlayableSkill> AvailableSkills { get; private set; } = new Dictionary<string, PlayableSkill>();

#if UNITY_EDITOR
    [ShowInInspector] List<string> _availableSkillIds => AvailableSkills.Keys.ToList();
    [ShowInInspector] List<PlayableSkill> _availableSkill => AvailableSkills.Values.ToList();
#endif

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
                if(!AvailableSkills.ContainsKey(key))
                    AvailableSkills.Add(key, new PlayableSkill(key));
            }
        }
    }

    public Skill LoopBasicAttack()
    {
        if (_animationDatabase == null) return null;
        if (_skillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;

        foreach (string key in WeaponSkillSO.SkillDict[0])
        {
            if (!AvailableSkills[key].IsInCooldown)
                if (_skillDatabase.SkillDict.ContainsKey(AvailableSkills[key].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(_skillDatabase.SkillDict[AvailableSkills[key].SkillId].AnimationID))
                {
                    StartCoroutine(SkillCooldownCO(key, _skillDatabase.SkillDict[AvailableSkills[key].SkillId].Cooldown));
                    return _skillDatabase.SkillDict[AvailableSkills[key].SkillId];
                }
        }
        return null;
    }

    public AnimationClip GetAnimationClip(string animationId)
    {
        return _animationDatabase.SkillAnimDict[animationId];
    }

    private IEnumerator SkillCooldownCO(string key, float cooldownTime)
    {
        AvailableSkills[key].IsInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        AvailableSkills[key].IsInCooldown = false;
    }
}
