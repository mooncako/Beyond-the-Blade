//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Settings;
using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Skill Web logger.
    /// </summary>
    public static class SkillWebLogger
    {
        /// <summary>
        /// Logs a message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log(object message)
        {
            if (SkillWeb.Settings.debugLogMode == SkillWebSettings.DebugLogMode.Normal)
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// Logs a warning message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning(object message)
        {
            if (SkillWeb.Settings.debugLogMode == SkillWebSettings.DebugLogMode.Normal)
            {
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Logs an error message to the Unity console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError(object message)
        {
            if (SkillWeb.Settings.debugLogMode == SkillWebSettings.DebugLogMode.Normal)
            {
                Debug.LogError(message);
            }
        }
    }
}