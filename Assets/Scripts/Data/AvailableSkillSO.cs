using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Weapon", menuName = "BytheBlade/AvailableSkillSO")]
public class AvailableSkillSO : SerializedScriptableObject
{   
    [Header("SkillKey")]
    public Dictionary<int, List<string>> SkillDict = new Dictionary<int, List<string>>();
}
