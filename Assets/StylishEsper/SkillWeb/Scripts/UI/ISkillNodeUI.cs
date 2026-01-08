//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using Esper.SkillWeb.UI.UGUI;

namespace Esper.SkillWeb.UI
{
    /// <summary>
    /// Interface for a Skill Web runtime skill node.
    /// </summary>
    public interface ISkillNodeUI
    {
        /// <summary>
        /// Sets the skill node that this object represents.
        /// </summary>
        /// <param name="skillNode">The skill node to represent.</param>
        /// <param name="webView">The web view reference.</param>
        public abstract void SetSkill(SkillNode skillNode, WebViewUGUI webView);

        /// <summary>
        /// Resets the skill level to 0.
        /// </summary>
        public abstract void ResetLevel();

        /// <summary>
        /// Refreshes this UI object.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Increases the level by 1 if possible. The skill must be unlocked and the level cannot be higher than the 
        /// max level.
        /// </summary>
        /// <returns>True if the skill was leveled up. Otherwise, false.</returns>
        public abstract bool TryUpgrade();

        /// <summary>
        /// Decreases the level by 1 if possible. The level cannot be lower than 0.
        /// </summary>
        /// <returns>True if the skill level was decreased. Otherwise, false.</returns>
        public abstract bool TryDowngrade();
    }
}