using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Weapon", menuName = "BytheBlade/AvailableSkillSO")]
public class AvailableSkillSO : SerializedScriptableObject
{   
    [Header("SkillKey")]
    public Dictionary<AvailableSkillType, List<string>> SkillDict = new Dictionary<AvailableSkillType, List<string>>();
}
