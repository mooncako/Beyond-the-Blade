//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Editor;
using System.IO;
#endif
using UnityEngine;

namespace Esper.SkillWeb
{
    /// <summary>
    /// A dataset that can be given to skills. This is meant to be a parent class of your own dataset object.
    /// </summary>
    public abstract class SkillDataset : SkillWebObject
    {
        /// <summary>
        /// The path to all generated objects of this type relative to the resources folder.
        /// </summary>
        public static string resourcesPath = "SkillWeb/Skills/GeneratedDatasets";

#if UNITY_EDITOR
        /// <summary>
        /// The directory of all generated objects of this type. Works in the editor only.
        /// </summary>
        public static string DirectoryPath { get => Path.Combine(AssetSearch.FindFolder("SkillWeb"), "Resources", "SkillWeb", "Skills", "GeneratedDatasets"); }
#endif

        /// <summary>
        /// The skill reference.
        /// </summary>
        [HideInInspector]
        public Skill skill;

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        /// <returns>The skill name.</returns>
        public abstract string GetName();

        /// <summary>
        /// Gets the skill subtext.
        /// </summary>
        /// <returns>The skill subtext.</returns>
        public abstract string GetSubtext();

        /// <summary>
        /// Gets the skill description.
        /// </summary>
        /// <returns>The skill description.</returns>
        public abstract string GetDescription();
    }
}