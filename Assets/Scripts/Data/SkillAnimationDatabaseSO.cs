using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillAnimationDatabase", menuName = "BytheBlade/SkillAnimationDatabase")]
public class SkillAnimationDatabaseSO : SerializedScriptableObject
{
    public Dictionary<string, AnimationClip> SkillAnimDict = new Dictionary<string, AnimationClip>();
}
