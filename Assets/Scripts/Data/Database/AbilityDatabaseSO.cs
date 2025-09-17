using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "BytheBlade/AbilityDatabase")]
public class AbilityDatabaseSO : ScriptableObject
{
    [SerializeField] private PlayerSkillsSO _playerSkillDatabase;
    public List<PoolableSkill> Abilities = new List<PoolableSkill>();
    public List<PoolableSkill> NormalAbilities = new List<PoolableSkill>();
    public List<PoolableSkill> RareAbilities = new List<PoolableSkill>();
    public List<PoolableSkill> EpicAbilities = new List<PoolableSkill>();
    public List<PoolableSkill> LegendaryAbilities = new List<PoolableSkill>();



    [Button]
    public void UpdateProbabilities()
    {

        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Abilities[i].CanAppear)
            {
                switch (_playerSkillDatabase.SkillDict[Abilities[i].SkillId].Rarity)
                {
                    case Rarity.Normal:
                        NormalAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Rare:
                        RareAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Epic:
                        EpicAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Legendary:
                        LegendaryAbilities.Add(Abilities[i]);
                        break;
                }
            }
        }
    }
}
