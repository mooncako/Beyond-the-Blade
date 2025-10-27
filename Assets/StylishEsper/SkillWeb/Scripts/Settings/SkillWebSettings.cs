//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Esper.SkillWeb.Settings
{
    /// <summary>
    /// Skill Web's settings.
    /// </summary>
    public class SkillWebSettings : ScriptableObject
    {
        /// <summary>
        /// Controls the debug messages logged in the console.
        /// </summary>
        public DebugLogMode debugLogMode = DebugLogMode.Normal;

        /// <summary>
        /// If the player level requirement for unlocking skills should be enabled.
        /// </summary>
        public bool enablePlayerLevelRequirement = true;

        /// <summary>
        /// If skill downgrading should be enabled.
        /// </summary>
        public bool enableDowngrading = true;

        /// <summary>
        /// The delta time mode.
        /// </summary>
        public DeltaTime deltaTime;

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string databaseName = "SkillWeb";

        /// <summary>
        /// The skill dataset reference. Used by all skills to allow custom data in the skill bank editor.
        /// </summary>
        public SkillDataset skillDatasetReference;

        /// <summary>
        /// The web dataset reference. Used by all webs to allow custom data in the web creator.
        /// </summary>
        public WebDataset webDatasetReference;

        /// <summary>
        /// A list of all skill tags.
        /// </summary>
        public List<string> tags = new();

        /// <summary>
        /// The zooming strength.
        /// </summary>
        public float zoomStrength = 0.2f;

        /// <summary>
        /// Smooths zooming. Higher = faster smoothing.
        /// </summary>
        public float zoomSmoothing = 8f;

        /// <summary>
        /// The minimum zoom scale.
        /// </summary>
        public float minScale = 0.25f;

        /// <summary>
        /// The maximum zoom scale.
        /// </summary>
        public float maxScale = 1.5f;

        /// <summary>
        /// The scale that the graph starts with when first loaded.
        /// </summary>
        public StartingScale startingScale;

        /// <summary>
        /// Smooths panning. Higher = faster smoothing.
        /// </summary>
        public float panSmoothing = 8f;

        /// <summary>
        /// The amount of time control is disabled after a node has been snapped to.
        /// </summary>
        public float afterSnapDelay = 0.25f;

        /// <summary>
        /// If panning bounds should be automatically calculated and applied.
        /// </summary>
        public bool automaticBoundsEnabled = true;

        /// <summary>
        /// The speed used when resetting the view or focusing. Speed at 1 will take 1s to reset.
        /// </summary>
        public float resetFocusSpeed = 5f;

        /// <summary>
        /// The button used to drag/pan the web view.
        /// </summary>
        public PointerEventData.InputButton mousePanButton = PointerEventData.InputButton.Middle;

        /// <summary>
        /// Node size settings.
        /// </summary>
        public SkillNodeSizes skillNodeSizes;

        /// <summary>
        /// Gets the interval in seconds from the last frame to the current one based on the DeltaTime mode.
        /// </summary>
        /// <returns>The delta time.</returns>
        public float GetDeltaTime()
        {
            switch (deltaTime)
            {
                case DeltaTime.DeltaTime:
                    return Time.deltaTime;

                case DeltaTime.UnscaledDeltaTime:
                    return Time.unscaledDeltaTime;

                default:
                    return Time.deltaTime;
            }
        }

        /// <summary>
        /// Node size values.
        /// </summary>
        [Serializable]
        public struct SkillNodeSizes
        {
            /// <summary>
            /// The size for tiny skill nodes.
            /// </summary>
            public float tiny;

            /// <summary>
            /// The size for small skill nodes.
            /// </summary>
            public float small;

            /// <summary>
            /// The size for medium skill nodes.
            /// </summary>
            public float medium;

            /// <summary>
            /// The size for large skill nodes.
            /// </summary>
            public float large;

            /// <summary>
            /// The size for giant skill nodes.
            /// </summary>
            public float giant;
        }

        /// <summary>
        /// Supported log modes.
        /// </summary>
        public enum DebugLogMode
        {
            /// <summary>
            /// No logs.
            /// </summary>
            None,

            /// <summary>
            /// Normal logs.
            /// </summary>
            Normal
        }

        /// <summary>
        /// Web view starting scale types.
        /// </summary>
        public enum StartingScale
        {
            /// <summary>
            /// Start at the minimum scale.
            /// </summary>
            Min,

            /// <summary>
            /// Start at the normal scale (1).
            /// </summary>
            Normal,

            /// <summary>
            /// Start at the maximum scale.
            /// </summary>
            Max
        }

        /// <summary>
        /// Supported delta time modes.
        /// </summary>
        public enum DeltaTime
        {
            /// <summary>
            /// The delta time mode which is affected by Time.timeScale.
            /// </summary>
            [InspectorName("Delta Time")]
            DeltaTime,

            /// <summary>
            /// The delta time mode which is unaffected by Time.timeScale.
            /// </summary>
            [InspectorName("Unscaled Delta Time")]
            UnscaledDeltaTime
        }
    }
}