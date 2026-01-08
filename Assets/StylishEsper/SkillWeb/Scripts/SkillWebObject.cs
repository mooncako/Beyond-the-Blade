//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// The parent class of Skill Web's generated objects.
    /// </summary>
    public abstract class SkillWebObject : ScriptableObject
    {
        /// <summary>
        /// The object's unique ID.
        /// </summary>
        [HideInInspector]
        public int id;

        /// <summary>
        /// Returns a sanitized version of the original name by removing unaccepted characters.
        /// </summary>
        /// <param name="original">The original name.</param>
        /// <returns>The sanitized name.</returns>
        public virtual string SanitizeName(string original)
        {
            char[] invalidChars = { '/', '?', '<', '>', '\\', '|', '*', '?', '"' };
            string sanitized = new string(original.Where(c => !invalidChars.Contains(c)).ToArray());
            return sanitized;
        }

        /// <summary>
        /// Saves the object (editor only).
        /// </summary>
        public virtual void Save()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
            };
#endif
        }

        /// <summary>
        /// Returns an unused ID.
        /// </summary>
        /// <typeparam name="T">The type. Must be a SkillWebObject.</typeparam>
        /// <param name="pathInResources">The path in the resources folder.</param>
        /// <returns>An unused ID.</returns>
        protected virtual int GetID<T>(string pathInResources = null) where T : SkillWebObject
        {
            if (string.IsNullOrEmpty(pathInResources))
            {
                return -1;
            }

            var items = Resources.LoadAll<T>(pathInResources);
            var ids = new List<int>();

            foreach (var item in items)
            {
                ids.Add(item.id);
            }

            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!ids.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}