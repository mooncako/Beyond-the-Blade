using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkillsDatabase", menuName = "BytheBlade/EnemySkillsDatabase")]
public class EnemySkillsSO : SerializedScriptableObject
{
    public Dictionary<string, Skill> EnemySkillDict = new Dictionary<string, Skill>();
}
