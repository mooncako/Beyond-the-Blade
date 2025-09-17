using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolableModifierDatabase", menuName = "BytheBlade/PoolableModifierDatabase")]
public class PoolableModifierDatabaseSO : ScriptableObject
{
    [SerializeField] private ModifierDatabaseSO _modifierDatabase;
    public List<PoolableSkill> Modifiers = new List<PoolableSkill>();

    public List<PoolableSkill> NormalModifiers = new List<PoolableSkill>();
    public List<PoolableSkill> RareModifiers = new List<PoolableSkill>();
    public List<PoolableSkill> EpicModifiers = new List<PoolableSkill>();
    public List<PoolableSkill> LegendaryModifiers = new List<PoolableSkill>();



    [Button]
    public void UpdateProbabilities()
    {

        for (int i = 0; i < Modifiers.Count; i++)
        {
            if (Modifiers[i].CanAppear)
            {
                switch (_modifierDatabase.SkillModifierDict[Modifiers[i].SkillId].Rarity)
                {
                    case Rarity.Normal:
                        NormalModifiers.Add(Modifiers[i]);
                        break;
                    case Rarity.Rare:
                        RareModifiers.Add(Modifiers[i]);
                        break;
                    case Rarity.Epic:
                        EpicModifiers.Add(Modifiers[i]);
                        break;
                    case Rarity.Legendary:
                        LegendaryModifiers.Add(Modifiers[i]);
                        break;
                }
            }
        }
    }
}
