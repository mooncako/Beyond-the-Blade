using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "BytheBlade/AbilityDatabase")]
public class AbilityDatabaseSO : ScriptableObject
{
    [SerializeField] private PlayerSkillsSO _playerSkillDatabase;
    public List<PooledAbility> Abilities = new List<PooledAbility>();

    [Button]
    public void UpdateProbabilities()
    {
        List<PooledAbility> normalAbilities = new List<PooledAbility>();
        List<PooledAbility> rareAbilities = new List<PooledAbility>();
        List<PooledAbility> epicAbilities = new List<PooledAbility>();
        List<PooledAbility> legendaryAbilities = new List<PooledAbility>();

        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Abilities[i].CanAppear)
            {
                switch (_playerSkillDatabase.SkillDict[Abilities[i].SkillId].Rarity)
                {
                    case Rarity.Normal:
                        normalAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Rare:
                        rareAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Epic:
                        epicAbilities.Add(Abilities[i]);
                        break;
                    case Rarity.Legendary:
                        legendaryAbilities.Add(Abilities[i]);
                        break;
                }
            }
            else
            {
                Abilities[i].PossibilityIndex = 0;
            }
        }

        float currentPossibility = 0;
        for (int i = 0; i < normalAbilities.Count; i++)
        {
            currentPossibility += 1f / normalAbilities.Count;
            normalAbilities[i].PossibilityIndex = currentPossibility;
        }

        currentPossibility = 0;
        for (int i = 0; i < rareAbilities.Count; i++)
        {
            currentPossibility += 1f / rareAbilities.Count;
            rareAbilities[i].PossibilityIndex = currentPossibility;
        }

        currentPossibility = 0;
        for (int i = 0; i < epicAbilities.Count; i++)
        {
            currentPossibility += 1f / epicAbilities.Count;
            epicAbilities[i].PossibilityIndex = currentPossibility;
        }

        currentPossibility = 0;
        for (int i = 0; i < legendaryAbilities.Count; i++)
        {
            currentPossibility += 1f / legendaryAbilities.Count;
            legendaryAbilities[i].PossibilityIndex = currentPossibility;
        }
    }
}
