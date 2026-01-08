//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Esper.SkillWeb.Procedural
{
    /// <summary>
    /// Settings used to generate Webs at runtime.
    /// </summary>
    public struct WebGenerationSettings
    {
        /// <summary>
        /// Skill node groups to generate.
        /// </summary>
        public List<SkillGroup> skillGroups;

        /// <summary>
        /// The connection algorithm to use.
        /// </summary>
        public ConnectionAlgorithm connectionAlgorithm;

        /// <summary>
        /// The skill sizes that are possible of being a starting skill.
        /// </summary>
        public List<Skill.Size> possibleStartingSkillSizes;

        /// <summary>
        /// Only the skills with a tag in this list will be included. Leave this empty if all tags should be allowed.
        /// </summary>
        public List<string> skillTags;

        /// <summary>
        /// The minimum distance away a focus point should be from all other skills. This is not a strict min. A focus point
        /// is a randomized center of a skill group.
        /// </summary>
        public float minPreferredFocusPointDistance;

        /// <summary>
        /// The maximum distance away a focus point should be from all other nodes. This is not a strict max. A focus point
        /// is a randomized center of a skill group.
        /// </summary>
        public float maxPreferredFocusPointDistance;

        /// <summary>
        /// The min number of skills that aren't connection dependant.
        /// </summary>
        public int minStartingSkills;

        /// <summary>
        /// The max number of skills that aren't connection dependant.
        /// </summary>
        public int maxStartingSkills;

        /// <summary>
        /// The max number of connections each skill can have.
        /// </summary>
        public int maxPossibleConnectionsPerSkill;

        /// <summary>
        /// The connection style index for a connection that connects to a tiny skill.
        /// </summary>
        public int connectionToTinyStyleIndex;

        /// <summary>
        /// The connection style index for a connection that connects to a small skill.
        /// </summary>
        public int connectionToSmallStyleIndex;

        /// <summary>
        /// The connection style index for a connection that connects to a medium skill.
        /// </summary>
        public int connectionToMediumStyleIndex;

        /// <summary>
        /// The connection style index for a connection that connects to a large skill.
        /// </summary>
        public int connectionToLargeStyleIndex;

        /// <summary>
        /// The connection style index for a connection that connects to a giant skill.
        /// </summary>
        public int connectionToGiantStyleIndex;

        /// <summary>
        /// The center point of the nodes.
        /// </summary>
        public Vector2 centerPoint;

        /// <summary>
        /// Represents a group of nodes to generate.
        /// </summary>
        public struct SkillGroup
        {
            /// <summary>
            /// The total number of skill nodes to generate.
            /// </summary>
            public int count;

            /// <summary>
            /// A function used by the randomization algorithm to filter out unwanted skills. If this is null, any skill can
            /// be selected.
            /// </summary>
            public Func<Skill, bool> filter;

            /// <summary>
            /// The minimum distance away a each skill in this group should be from all other skills. This is not a strict min.
            /// </summary>
            public float minPreferredDistance;

            /// <summary>
            /// The maximum distance away a each skill in this group should be from all other skills. This is not a strict max.
            /// </summary>
            public float maxPreferredDistance;

            /// <summary>
            /// If duplicate skills should be disallowed for this skill group. Setting this to true may result in the count
            /// value being disregarded if no possible unique skills remain.
            /// </summary>
            public bool uniqueNodes;

            /// <summary>
            /// The position search algorithm settings.
            /// </summary>
            public PositionSearch positionSearch;
        }

        public struct PositionSearch
        {
            /// <summary>
            /// Base angular step in degrees for spiral iterations.
            /// </summary>
            public float angleStep;

            /// <summary>
            /// Base growth factor for spiral radius.
            /// </summary>
            public float spiralGrowth;

            /// <summary>
            /// Minimum and maximum bounds for randomized angle step.
            /// </summary>
            public Vector2 angleStepRange;

            /// <summary>
            /// Minimum and maximum bounds for randomized spiral growth.
            /// </summary>
            public Vector2 spiralGrowthRange;

            /// <summary>
            /// Whether to randomize angle step and spiral growth within defined ranges.
            /// </summary>
            public bool randomize;

            /// <summary>
            /// Gets an actual angle step (randomized if enabled).
            /// </summary>
            public float GetAngleStep()
            {
                return randomize ? UnityEngine.Random.Range(angleStepRange.x, angleStepRange.y) : angleStep;
            }

            /// <summary>
            /// Gets an actual spiral growth value (randomized if enabled).
            /// </summary>
            public float GetSpiralGrowth()
            {
                return randomize ? UnityEngine.Random.Range(spiralGrowthRange.x, spiralGrowthRange.y) : spiralGrowth;
            }

            /// <summary>
            /// Returns a default configuration.
            /// </summary>
            public static PositionSearch Default => new PositionSearch
            {
                angleStep = 10f,
                spiralGrowth = 30f,
                angleStepRange = new Vector2(10f, 30f),
                spiralGrowthRange = new Vector2(30f, 90f),
                randomize = true
            };
        }

        /// <summary>
        /// Web generation connection algorithm types.
        /// </summary>
        public enum ConnectionAlgorithm
        {
            /// <summary>
            /// Generally starts from the smaller skills and connects to larger ones.
            /// </summary>
            SmallToLarge,

            /// <summary>
            /// Generally starts from the largest skills and connects to smaller ones.
            /// </summary>
            LargeToSmall,

            /// <summary>
            /// Randomly connects skills.
            /// </summary>
            Random
        }
    }
}