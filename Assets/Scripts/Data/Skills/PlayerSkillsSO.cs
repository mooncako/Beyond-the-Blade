using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillsDatabase", menuName = "BytheBlade/PlayerSkillsDatabase")]
public class PlayerSkillsSO : SkillsSO
{
#if UNITY_EDITOR
    [Button]
    private void Copy(PlayerSkillsSO skills)
    {
        if (skills != null)
        {
            foreach (KeyValuePair<string, Skill> entry in skills.SkillDict)
            {
                SkillDict[entry.Key] = new Skill(entry.Value);
            }
        }

    }
#endif
}
