using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillAnimationDatabase", menuName = "BytheBlade/SkillAnimationDatabase")]
public class SkillAnimationDatabaseSO : SerializedScriptableObject
{
    public Dictionary<string, SkillAnimation> SkillAnimDict = new Dictionary<string, SkillAnimation>();
}
