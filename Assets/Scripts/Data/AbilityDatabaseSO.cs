using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "BytheBlade/AbilityDatabase")]
public class AbilityDatabaseSO : ScriptableObject
{
    [SerializeField] private PlayerSkillsSO _playerSkillDatabase;
    public List<PooledAbility> Abilities = new List<PooledAbility>();
    public List<PooledAbility> NormalAbilities = new List<PooledAbility>();
    public List<PooledAbility> RareAbilities = new List<PooledAbility>();
    public List<PooledAbility> EpicAbilities = new List<PooledAbility>();
    public List<PooledAbility> LegendaryAbilities = new List<PooledAbility>();



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
