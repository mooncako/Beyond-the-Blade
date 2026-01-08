//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System.Collections.Generic;

namespace Esper.SkillWeb.DataManagement
{
    /// <summary>
    /// A savable version of a Web.
    /// </summary>
    [System.Serializable]
    public class SavableWeb
    {
        /// <summary>
        /// The WebGraph ID. -1 is used for generated webs.
        /// </summary>
        public int id;

        /// <summary>
        /// A guid that was generated when this web was last changed.
        /// </summary>
        public string changeGuid;

        /// <summary>
        /// The list of all SkillNode's as SavableSkillNode's.
        /// </summary>
        public List<SavableSkillNode> savableSkillNodes = new();

        /// <summary>
        /// The list of all connections. Connections are already savable so there's no need for a savable version.
        /// </summary>
        public List<Connection> connections = new();

        /// <summary>
        /// Converts this SavableWeb back to a regular Web.
        /// </summary>
        /// <returns>The SavableWeb deconverted.</returns>
        public Web ToWeb()
        {
            var web = new Web(this);
            return web;
        }

        /// <summary>
        /// Gets the sum of the level of all skills.
        /// </summary>
        /// <returns>The total combined level of all skills.</returns>
        public int SkillLevelTotal()
        {
            int total = 0;

            foreach (var skill in savableSkillNodes)
            {
                total += skill.level;
            }

            return total;
        }
    }
}