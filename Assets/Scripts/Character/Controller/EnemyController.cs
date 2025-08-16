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
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private Skill _currentSkill;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _targetPos;

    private List<GameObject> _hitTargets = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        AvailableSkills.Sort((a, b) => b.BaseWeight.CompareTo(a.BaseWeight));
    }

    protected override void OnEnable()
    {
        
    }

    public void MoveTo(Vector3 destination)
    {
        if (CanMove)
        {
            Movement.MoveTo(destination);
        }
        else
        {
            Movement.Stop();
        }
    }

    public void Stop()
    {
        Movement.Stop();
    }

    public void ActivateSkill()
    {
        if (_animationDatabase == null) return;
        if (_skillsDatabase == null) return;
        if (AvailableSkills.Count == 0) return;
        _currentSkill = null;

        // Swap out the animation on the attack state, then makes it go under cooldown
        for (int i = 0; i < AvailableSkills.Count; i++)
        {
            if (!AvailableSkills[i].IsInCooldown)
            {
                if (_skillsDatabase.EnemySkillDict.ContainsKey(AvailableSkills[i].SkillId) &&
                    _animationDatabase.SkillAnimDict.ContainsKey(_skillsDatabase.EnemySkillDict[AvailableSkills[i].SkillId].AnimationID))
                {
                    _currentSkill = _skillsDatabase.EnemySkillDict[AvailableSkills[i].SkillId];
                    ApplySkillEffect();
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

    private void ApplySkillEffect()
    {
        //TODO buffs & debuffs
        AOEApplier.X = _currentSkill.SkillRange.X;
        AOEApplier.Y = _currentSkill.SkillRange.Y;
        AOEApplier.Z = _currentSkill.SkillRange.Z;
        _hitTargets.Clear();
        
    }

    public void DamageAnimEvent()
    {
        if (_currentSkill.IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, _targetPos);
        }
        else
        {
            if (AttackPoint != null)
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, AttackPoint.position);
            }
            else
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, transform.position);
            }
        }


        foreach (GameObject target in _hitTargets)
        {
            DamageInfo info = new DamageInfo(_currentSkill.Damage, target, gameObject, gameObject, DamageType.Regular);
            target.GetComponent<Health>().Damage(info);
        }
    }

    public void SetTargetPos(Vector3 position)
    {
        _targetPos = position;
    }
    
}
