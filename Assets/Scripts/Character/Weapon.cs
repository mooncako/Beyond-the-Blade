using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Data")] private SkillAnimationDatabaseSO _animationDatabase;
    [SerializeField, BoxGroup("Data")] private SkillsSO _skillDatabase;
    [field: SerializeField, BoxGroup("Skills")] public List<PlayableSkill> AvailableSkills { get; private set; } = new List<PlayableSkill>();

    void Awake()
    {
        AvailableSkills.Sort((a, b) => b.BaseWeight.CompareTo(a.BaseWeight));
    }

    void OnEnable()
    {
        AvailableSkills.Sort((a, b) => b.BaseWeight.CompareTo(a.BaseWeight));
    }

    public Skill GetAvailableSkill()
    {
        if (_animationDatabase == null) return null;
        if (_skillDatabase == null) return null;
        if (AvailableSkills.Count == 0) return null;


        // Swap out the animation on the attack state, then makes it go under cooldown
        for (int i = 0; i < AvailableSkills.Count; i++)
        {
            if (!AvailableSkills[i].IsInCooldown)
            {
                if (_skillDatabase.SkillDict.ContainsKey(AvailableSkills[i].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(_skillDatabase.SkillDict[AvailableSkills[i].SkillId].AnimationID))
                {
                    StartCoroutine(SkillCooldownCO(i, _skillDatabase.SkillDict[AvailableSkills[i].SkillId].Cooldown));
                    return _skillDatabase.SkillDict[AvailableSkills[i].SkillId];
                }

            }
        }

        return null;
    }

    public ClipTransition GetAnimationClip(string animationId)
    {
        return _animationDatabase.SkillAnimDict[animationId];
    }

    private IEnumerator SkillCooldownCO(int index, float cooldownTime)
    {
        AvailableSkills[index].IsInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        AvailableSkills[index].IsInCooldown = false;
    }
}
