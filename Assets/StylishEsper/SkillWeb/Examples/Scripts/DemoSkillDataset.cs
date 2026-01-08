//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// The dataset used for Skill Web's demos.
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill Dataset", menuName = "Skill Web/Datasets/Demo Skill Dataset")]
    public class DemoSkillDataset : DefaultSkillDataset
    {
        /// <summary>
        /// The cost to upgrade the skill.
        /// </summary>
        public float cost;
    }
}