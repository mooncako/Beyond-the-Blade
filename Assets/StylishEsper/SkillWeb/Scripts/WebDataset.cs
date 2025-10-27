//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Editor;
using System.IO;
#endif
using Esper.SkillWeb.Graph;
using UnityEngine;

namespace Esper.SkillWeb
{
    public abstract class WebDataset : SkillWebObject
    {
        /// <summary>
        /// The path to all generated objects of this type relative to the resources folder.
        /// </summary>
        public static string resourcesPath = "Skill Web/Webs/GeneratedDatasets";

#if UNITY_EDITOR
        /// <summary>
        /// The directory of all generated objects of this type. Works in the editor only.
        /// </summary>
        public static string DirectoryPath { get => Path.Combine(AssetSearch.FindFolder("SkillWeb"), "Resources", "SkillWeb", "Webs", "GeneratedDatasets"); }
#endif

        /// <summary>
        /// The web graph reference.
        /// </summary>
        [HideInInspector]
        public WebGraph graph;

        /// <summary>
        /// Gets the web graph's display name.
        /// </summary>
        /// <returns>The web graph's display name.</returns>
        public abstract string GetDisplayName();
    }
}