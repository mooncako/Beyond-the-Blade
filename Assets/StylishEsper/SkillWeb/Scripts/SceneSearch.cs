//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb
{
    public static class SceneSearch
    {
        /// <summary>
        /// Finds the first object of a given type in the scene.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The object or null if one of the type was not found in the scene.</returns>
        public static T FindFirstInScene<T>() where T : Object
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
            return Object.FindObjectOfType<T>(true);
#endif
        }
    }
}