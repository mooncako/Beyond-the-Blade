using System.Collections.Generic;
using UnityEngine;

public static class AbilityDatabaseUtil
{
    public static void UpdateProbabilities(List<PooledAbility> abilities, Dictionary<string, Skill> skillDict)
    {
        List<PooledAbility> normalAbilities = new List<PooledAbility>();
        List<PooledAbility> rareAbilities = new List<PooledAbility>();
        List<PooledAbility> epicAbilities = new List<PooledAbility>();
        List<PooledAbility> legendaryAbilities = new List<PooledAbility>();

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].CanAppear)
            {
                switch (skillDict[abilities[i].SkillId].Rarity)
                {
                    case Rarity.Normal:
                        normalAbilities.Add(abilities[i]);
                        break;
                    case Rarity.Rare:
                        rareAbilities.Add(abilities[i]);
                        break;
                    case Rarity.Epic:
                        epicAbilities.Add(abilities[i]);
                        break;
                    case Rarity.Legendary:
                        legendaryAbilities.Add(abilities[i]);
                        break;
                }
            }
            else
            {
                abilities[i].PossibilityIndex = 0;
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
