//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;

namespace Esper.SkillWeb.Attributes
{
    /// <summary>
    /// An attribute that marks a field of a custom SkillDataset as a base value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class BaseValueAttribute : Attribute
    {
        /// <summary>
        /// The attribute ID.
        /// </summary>
        public string id;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The attribute ID.</param>
        public BaseValueAttribute(string id) => this.id = id;
    }
}