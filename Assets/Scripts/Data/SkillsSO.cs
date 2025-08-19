using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillsSO : SerializedScriptableObject
{
    public Dictionary<string, Skill> SkillDict = new Dictionary<string, Skill>();
}
