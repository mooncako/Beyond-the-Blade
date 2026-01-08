//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Procedural;
using Esper.SkillWeb.UI.UGUI;
using System.Collections.Generic;
using UnityEngine;

namespace Esper.SkillWeb.Examples
{
    public class ProceduralDemoExample : MonoBehaviour
    {
        private void Start()
        {
            // Generate at start
            GenerateRandomWeb();
        }

        /// <summary>
        /// Generates a random web.
        /// </summary>
        public void GenerateRandomWeb()
        {
            // Create settings (set your own values)
            var settings = new WebGenerationSettings()
            {
                skillGroups = new List<WebGenerationSettings.SkillGroup>()
                {
                    // Create a group of skills (one group of 200 skills is created here)
                    new WebGenerationSettings.SkillGroup()
                    {
                        count = 100, // Number of total skills
                        filter = null, // null filter will allow any skill in this group
                        minPreferredDistance = 300, // Minimum distance from other skills
                        maxPreferredDistance = 600, // Maximum distance from other skills
                        uniqueNodes = false, // Allow the same node (skill) to be used multiple times
                        positionSearch = new WebGenerationSettings.PositionSearch() // Settings used to search for a position for the next skill
                        {
                            angleStep = 15f,
                            spiralGrowth = 15f,
                            angleStepRange = new Vector2(15f, 30f),
                            spiralGrowthRange = new Vector2(15f, 30f),
                            randomize = true
                        },

                    }
                },

                skillTags = new() { "Sword Mastery (Demo)" }, // Only allow skills tagged 'Sword Mastery (Demo)'
                connectionAlgorithm = WebGenerationSettings.ConnectionAlgorithm.SmallToLarge, // Generally connect small to large
                possibleStartingSkillSizes = new() { Skill.Size.Tiny, Skill.Size.Small }, // Only only tiny and small skills to be connection starters
                minPreferredFocusPointDistance = 500, // Min distance from other groups' focus point (there's only 1 group in this example)
                maxPreferredFocusPointDistance = 500, // Max distance from other groups' focus point
                minStartingSkills = 10, // Min number of starting skills
                maxStartingSkills = 20, // Max number of starting skills
                maxPossibleConnectionsPerSkill = 3, // Max connections a single skill can have.
                connectionToTinyStyleIndex = 0, // The connection prefab index used for any node connects to a tiny node
                connectionToSmallStyleIndex = 0, // The connection prefab index used for any node connects to a small node
                connectionToMediumStyleIndex = 0, // The connection prefab index used for any node connects to a medium node
                connectionToLargeStyleIndex = 0, // The connection prefab index used for any node connects to a large node
                connectionToGiantStyleIndex = 0, // The connection prefab index used for any node connects to a giant node
                centerPoint = Vector2.zero // The center point of the generated graph
            };

            // Generate the web
            var generatedWeb = WebGenerator.GenerateRandom(settings);

            // Load it (ensure there's a web view in your scene)
            WebViewUGUI.Active?.Load(generatedWeb);
        }
    }
}