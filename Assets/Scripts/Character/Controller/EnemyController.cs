using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyController : Controller
{
    [field: SerializeField, BoxGroup("Skills")] private SkillAnimationDatabaseSO _animationDatabase;
    [field: SerializeField, BoxGroup("Skills")] private EnemySkillsSO _skillsDatabase;
    [field: SerializeField, BoxGroup("Skills")] public List<PlayableSkill> AvailableSkills { get; private set; } = new List<PlayableSkill>();

    public bool CanMove = true;

    protected override void Awake()
    {
        base.Awake();
        AvailableSkills.Sort((a, b) => b.BaseWeight.CompareTo(a.BaseWeight));
    }

    [Button]
    public void ActivateSkill()
    {
        if (_animationDatabase == null) return;
        if (_skillsDatabase == null) return;
        if (AvailableSkills.Count == 0) return;



        for (int i = 0; i < AvailableSkills.Count; i++)
        {
            if (!AvailableSkills[i].IsInCooldown)
            {
                if (_skillsDatabase.EnemySkillDict.ContainsKey(AvailableSkills[i].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(_skillsDatabase.EnemySkillDict[AvailableSkills[i].SkillId].AnimationID))
                {
                    StartCoroutine(SkillCooldownCO(i, _skillsDatabase.EnemySkillDict[AvailableSkills[i].SkillId].Cooldown));
                    Animator.OverrideClipForState("Attack", _animationDatabase.SkillAnimDict[_skillsDatabase.EnemySkillDict[AvailableSkills[i].SkillId].AnimationID]);
                    Animator.Play("Attack");
                }

            }
        }

    }

    private IEnumerator SkillCooldownCO(int index, float cooldownTime)
    {
        AvailableSkills[index].IsInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        AvailableSkills[index].IsInCooldown = false;
    }
    
}
