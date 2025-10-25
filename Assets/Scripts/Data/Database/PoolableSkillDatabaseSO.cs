using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolableSkillDatabase", menuName = "BytheBlade/PoolableSkillDatabase")]
public class PoolableSkillDatabaseSO : ScriptableObject
{
    [SerializeField] private PlayerSkillsSO _playerSkillDatabase;
    public List<PoolableSkill> Skills = new List<PoolableSkill>();

    public List<PoolableSkill> NormalSkills = new List<PoolableSkill>();
    public List<PoolableSkill> RareSkills = new List<PoolableSkill>();
    public List<PoolableSkill> EpicSkills = new List<PoolableSkill>();
    public List<PoolableSkill> LegendarySkills = new List<PoolableSkill>();



    [Button]
    public void UpdateProbabilities()
    {

        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].CanAppear)
            {
                switch (_playerSkillDatabase.SkillDict[Skills[i].SkillId].Rarity)
                {
                    case Rarity.Normal:
                        NormalSkills.Add(Skills[i]);
                        break;
                    case Rarity.Rare:
                        RareSkills.Add(Skills[i]);
                        break;
                    case Rarity.Epic:
                        EpicSkills.Add(Skills[i]);
                        break;
                    case Rarity.Legendary:
                        LegendarySkills.Add(Skills[i]);
                        break;
                }
            }
        }
    }
}
