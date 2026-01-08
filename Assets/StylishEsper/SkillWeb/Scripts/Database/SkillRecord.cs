//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using SQLite;

namespace Esper.SkillWeb.Database
{
    /// <summary>
    /// Represents a single database skill record.
    /// </summary>
    public class SkillRecord
    {
        /// <summary>
        /// The skill ID.
        /// </summary>
        [Column("id")]
        public int id { get; set; }

        /// <summary>
        /// The name of the scriptable object.
        /// </summary>
        [Column("object_name")]
        public string objectName { get; set; }

        /// <summary>
        /// The skill name.
        /// </summary>
        [Column("skill_name")]
        public string skillName { get; set; }

        /// <summary>
        /// The skill size as an int.
        /// </summary>
        [Column("size")]
        public int size { get; set; }

        /// <summary>
        /// The skill tag.
        /// </summary>
        [Column("tag")]
        public string tag { get; set; }
    }
}