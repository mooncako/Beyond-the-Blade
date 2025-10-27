//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using UnityEngine;

namespace Esper.SkillWeb.DataManagement
{
    /// <summary>
    /// A savable version of a SkillNode.
    /// </summary>
    [System.Serializable]
    public class SavableSkillNode
    {
        /// <summary>
        /// The node ID.
        /// </summary>
        public int id;

        /// <summary>
        /// A guid generated when this node was initially created.
        /// </summary>
        public string guid;

        /// <summary>
        /// The skill ID.
        /// </summary>
        public int skillID;

        /// <summary>
        /// If the skill is connection dependant.
        /// </summary>
        public bool hasConnectionDependency;

        /// <summary>
        /// The position as a float array.
        /// </summary>
        public float[] position;

        /// <summary>
        /// The current level of the skill.
        /// </summary>
        public int level;

        /// <summary>
        /// Converts this SavableSkillNode back to a SkillNode.
        /// </summary>
        /// <returns>The SavableSkillNode deconverted.</returns>
        public SkillNode ToSkillNode()
        {
            var skill = SkillWeb.GetSkill(skillID);
            var skillNode = new SkillNode(id, skill, new Vector2(position[0], position[1]), new());
            skillNode.guid = guid;
            skillNode.hasConnectionDependency = hasConnectionDependency;
            skillNode.SetLevel(level, false);
            return skillNode;
        }
    }
}