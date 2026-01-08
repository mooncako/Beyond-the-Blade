//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;

namespace Esper.SkillWeb.Attributes
{
    /// <summary>
    /// An attribute that marks a field of a custom SkillDataset as a value that scales.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ScalerAttribute : Attribute
    {
        /// <summary>
        /// The attribute ID.
        /// </summary>
        public string id;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The attribute ID.</param>
        public ScalerAttribute(string id) => this.id = id;
    }
}