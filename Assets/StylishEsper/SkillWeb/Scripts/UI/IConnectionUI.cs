//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

namespace Esper.SkillWeb.UI
{
    /// <summary>
    /// Interface for a Skill Web runtime connection UI.
    /// </summary>
    public interface IConnectionUI
    {
        /// <summary>
        /// Refreshes this UI object.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Updates the start and end positions of the line.
        /// </summary>
        public abstract void UpdateLine();

        /// <summary>
        /// Highlights the connection.
        /// </summary>
        public abstract void Highlight();

        /// <summary>
        /// Unhighlights the connection.
        /// </summary>
        public abstract void Unhighlight();

        /// <summary>
        /// Visually updates the connection based on the current active state.
        /// </summary>
        public abstract void ApplyActiveState();
    }
}