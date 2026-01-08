//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;

namespace Esper.SkillWeb.Attributes
{
    /// <summary>
    /// An attribute that marks a field of a custom SkillDataset as a value used for scaling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ScalingAttribute : Attribute
    {
        /// <summary>
        /// The attribute ID.
        /// </summary>
        public string id;

        /// <summary>
        /// The value scaling mode.
        /// </summary>
        public ScalingMode mode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The attribute ID.</param>
        /// <param name="mode">The scaling mode.</param>
        public ScalingAttribute(string id, ScalingMode mode = ScalingMode.Linear)
        {
            this.id = id;
            this.mode = mode;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The attribute ID.</param>
        public ScalingAttribute(string id)
        {
            this.id = id;
            mode = ScalingMode.Curve;
        }
    }

    /// <summary>
    /// Value scaling modes.
    /// </summary>
    public enum ScalingMode
    {
        /// <summary>
        /// Linear scaling (base + scaling * level - 1).
        /// </summary>
        Linear,

        /// <summary>
        /// Ramped up scaling (base * scaling^(level - 1)).
        /// </summary>
        Exponential,

        /// <summary>
        /// Use an animation curve as the scaling source.
        /// </summary>
        Curve
    }
}