using UnityEngine;
using UnityEngine.Events;
using Animancer;
using Sirenix.OdinInspector;
public class SkillManager : MonoBehaviour
{

    [SerializeField] public PlayerSkillsSO PlayerSkillsSO;
    [SerializeField] public SkillAnimationDatabaseSO SkillAnimationDatabaseSO;
    [SerializeField] public Weapon CurrentWeapon;
    [SerializeField] public AnimationStateMachine _animationStateMachine;
    private AnimancerComponent _animancer;
    [ReadOnly, BoxGroup("Debug"), ShowInInspector]private Skill _currentSkill;
    private bool _isSkillPlaying;
    public UnityEngine.Events.UnityEvent OnSkillCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animancer = GetComponent<AnimancerComponent>();
        if(CurrentWeapon == null)
            CurrentWeapon = GetComponentInChildren<Weapon>();
        if (_animationStateMachine == null)
            _animationStateMachine = GetComponent<AnimationStateMachine>();
    }

    // Update is called once per frame
 

    public void GetAvailableSkill()
    {
        _currentSkill = CurrentWeapon.GetSkill(CurrentWeapon.WeaponSkillSO.SkillDict[2][0]);
    }

    public bool CanUseSKill(string skillId)
    {
        if (PlayerSkillsSO.SkillDict.ContainsKey(skillId))
        {
            return true;
        }
        return false;
    }

    public void UseSkill(string skillId)
    {
        if (_currentSkill == null) return;
        if (CanUseSKill(_currentSkill.AnimationID))
        {
            ExecuteSkill(_currentSkill.AnimationID);
        }
    }
    public void UseCurrentSkill()
    {
        GetAvailableSkill();
        if (_currentSkill == null) return;
        if (CanUseSKill(_currentSkill.AnimationID))
        {
            ExecuteSkill(_currentSkill.AnimationID);
        }
    }
    public void ExecuteSkill(string skillId)
    {
        if (SkillAnimationDatabaseSO.SkillAnimDict.ContainsKey(skillId))
        {
            _animationStateMachine.SetActionStateClip(CurrentWeapon.GetAnimationClip(skillId), AnimationStateType.Ability);
            _animationStateMachine.SwitchState(AnimationStateType.Ability);
        }
    }
}
    