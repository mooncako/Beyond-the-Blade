//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using SQLite;

namespace Esper.SkillWeb.Database
{
    /// <summary>
    /// Represents a single database skill record.
    /// </summary>
    public class WebRecord
    {
        /// <summary>
        /// The web ID.
        /// </summary>
        [Column("id")]
        public int id { get; set; }

        /// <summary>
        /// The name of the scriptable object.
        /// </summary>
        [Column("object_name")]
        public string objectName { get; set; }

        /// <summary>
        /// The web name.
        /// </summary>
        [Column("web_name")]
        public string webName { get; set; }
    }
}