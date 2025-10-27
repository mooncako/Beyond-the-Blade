//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Initializes Skill Web.
    /// </summary>
    public class SkillWebInitializer : MonoBehaviour
    {
        /// <summary>
        /// If the connection to Skill Web's database should be terminated on quit.
        /// </summary>
        [SerializeField]
        protected bool terminateConnectionOnQuit;

        private void Awake()
        {
            SkillWeb.Initialize(); 
        }

        private void OnApplicationQuit()
        {
            if (terminateConnectionOnQuit)
            {
                SkillWeb.Terminate();
            }
        }
    }
}