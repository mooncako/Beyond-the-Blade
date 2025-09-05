using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillModifierDatabase", menuName = "BytheBlade/SkillModifierDatabase")]
public class ModifierDatabaseSO : SerializedScriptableObject
{
    public Dictionary<string, ModifierSO> SkillModifierDict = new Dictionary<string, ModifierSO>();
}
