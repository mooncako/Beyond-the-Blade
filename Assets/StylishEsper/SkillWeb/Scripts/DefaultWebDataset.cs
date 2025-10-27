//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// The default dataset for all webs. It is recommended to create your own.
    /// </summary>
    [CreateAssetMenu(fileName = "New Web Dataset", menuName = "Skill Web/Datasets/Default Web Dataset")]
    public class DefaultWebDataset : WebDataset
    {
        /// <summary>
        /// The web's display name.
        /// </summary>
        public string displayName;

        public override string GetDisplayName()
        {
            return displayName;
        }
    }
}