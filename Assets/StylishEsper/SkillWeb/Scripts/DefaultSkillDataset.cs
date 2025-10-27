//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// The default dataset for all skills. It is recommended to create your own.
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill Dataset", menuName = "Skill Web/Datasets/Default Skill Dataset")]
    public class DefaultSkillDataset : SkillDataset
    {
        /// <summary>
        /// The skill type.
        /// </summary>
        public SkillType skillType;

        /// <summary>
        /// The skill's description.
        /// </summary>
        [TextArea]
        public string description;

        public override string GetDescription()
        {
            return description;
        }

        public override string GetName()
        {
            return skill.skillName;
        }

        public override string GetSubtext()
        {
            return skillType.ToString();
        }

        /// <summary>
        /// The type of skill.
        /// </summary>
        public enum SkillType
        {
            Passive,
            Active
        }
    }
}