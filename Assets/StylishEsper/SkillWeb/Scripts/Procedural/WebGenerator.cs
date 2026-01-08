//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Esper.SkillWeb.Procedural
{
    /// <summary>
    /// Runtime procedural Web generator.
    /// </summary>
    public static class WebGenerator
    {
        /// <summary>
        /// Creates a Web from a randomly selected existing WebGraph.
        /// </summary>
        /// <returns>The created Web.</returns>
        public static Web CreateFromRandomExisting()
        {
            var allWebs = SkillWeb.GetAllWebGraphs();

            if (allWebs.Length == 0)
            {
                SkillWebLogger.LogError("Web Generator: failed to create a Web. No WebGraph's exist.");
                return null;
            }

            var random = allWebs[Random.Range(0, allWebs.Length)];
            return new Web(random);
        }

        /// <summary>
        /// Generates a random WebGraph with default web generation settings.
        /// </summary>
        /// <param name="minPrefferedFocusPointDistance">The minimum distance away a focus point should be from all other nodes.</param>
        /// <param name="maxPreferredFocusPointDistance">The maximum distance away a focus point should be from all other nodes.</param>
        /// <param name="minStartingNodes">The min number of nodes that aren't connection dependant.</param>
        /// <param name="maxStartingNodes">The max number of nodes that aren't connection dependant.</param>
        /// <returns>The generated WebGraph.</returns>
        public static WebGraph GenerateRandom(float minPrefferedFocusPointDistance = 400, float maxPreferredFocusPointDistance = 1200, int minStartingNodes = -1, int maxStartingNodes = -1)
        {
            var possibleStartingNodeSizes = new List<Skill.Size>()
            {
                Skill.Size.Tiny,
                Skill.Size.Small
            };

            // Create default values based on number of skills
            var allSkills = SkillWeb.GetAllSkillsGroupedBySize();

            // Count possible starting nodes and all nodes
            int possibleStartingNodeCount = 0;
            int allNodeCount = 0;

            foreach (var size in allSkills.Keys)
            {
                if (possibleStartingNodeSizes.Contains(size))
                {
                    possibleStartingNodeCount += allSkills[size].Count;
                }

                allNodeCount += allSkills[size].Count;
            }

            int max = maxStartingNodes;
            int min = minStartingNodes;

            // If max and min starting nodes were not specified, make up to 10% of possible starting nodes the actual starting
            // node amount (min is half)
            if (maxStartingNodes == -1)
            {
                max = Mathf.CeilToInt(possibleStartingNodeCount * 0.1f);
            }

            if (minStartingNodes == -1)
            {
                min = Mathf.CeilToInt(max / 2f);
            }

            // Use default settings
            var settings = new WebGenerationSettings()
            {
                skillGroups = new List<WebGenerationSettings.SkillGroup>()
                {
                    new WebGenerationSettings.SkillGroup()
                    {
                        count = allNodeCount,
                        filter = null,
                        minPreferredDistance = 300,
                        maxPreferredDistance = 600,
                        uniqueNodes = false
                    }
                },

                connectionAlgorithm = WebGenerationSettings.ConnectionAlgorithm.SmallToLarge,
                skillTags = null,
                possibleStartingSkillSizes = possibleStartingNodeSizes,
                minPreferredFocusPointDistance = minPrefferedFocusPointDistance,
                maxPreferredFocusPointDistance = maxPreferredFocusPointDistance,
                minStartingSkills = min,
                maxStartingSkills = max,
                maxPossibleConnectionsPerSkill = 3,
                connectionToTinyStyleIndex = 0,
                connectionToSmallStyleIndex = 0,
                connectionToMediumStyleIndex = 0,
                connectionToLargeStyleIndex = 0,
                connectionToGiantStyleIndex = 0,
                centerPoint = Vector2.zero
            };

            return GenerateRandom(settings);
        }

        /// <summary>
        /// Generates a random WebGraph based on the web generation settings.
        /// </summary>
        /// <param name="settings">The web generation settings.</param>
        /// <returns>The generated WebGraph.</returns>
        public static WebGraph GenerateRandom(WebGenerationSettings settings)
        {
            if (settings.minStartingSkills <= 0)
            {
                settings.minStartingSkills = 1;
            }

            if (settings.maxStartingSkills <= 0)
            {
                settings.maxStartingSkills = 1;
            }

            if (settings.maxPossibleConnectionsPerSkill <= 0)
            {
                settings.maxPossibleConnectionsPerSkill = 1;
            }

            // Create graph
            var graph = ScriptableObject.CreateInstance<WebGraph>();
            graph.id = -9999;
            graph.GenerateDataset();

            var allSkills = SkillWeb.GetAllSkills();

            // List possible starting skills
            var possibleStarterNodes = new List<SkillNode>();
            var usedPositions = new List<Vector2>();
            var existingConnections = new List<Vector2[]>();

            // Loop through all groups
            foreach (var skillGroup in settings.skillGroups)
            {
                // Filter unwanted skills
                var filtered = allSkills.Where(x => (settings.skillTags == null || settings.skillTags.Count == 0 || settings.skillTags.Contains(x.Tag)) && (skillGroup.filter == null || skillGroup.filter(x))).ToList();

                // Skip if no skills match
                if (filtered.Count == 0)
                {
                    continue;
                }

                // Randomly select focus point
                var focusPoint = GenerateNodePosition(skillGroup.positionSearch, Vector2.zero, settings.minPreferredFocusPointDistance, settings.maxPreferredFocusPointDistance, usedPositions, existingConnections);
                usedPositions.Add(focusPoint);

                // Get focused skill based on connection algorithm type
                Skill focusedSkill;

                switch (settings.connectionAlgorithm)
                {
                    case WebGenerationSettings.ConnectionAlgorithm.SmallToLarge:
                        focusedSkill = filtered.SelectRandomBySize(Skill.Size.Tiny);
                        break;

                    case WebGenerationSettings.ConnectionAlgorithm.LargeToSmall:
                        focusedSkill = filtered.SelectRandomBySize(Skill.Size.Giant);
                        break;

                    default:
                    case WebGenerationSettings.ConnectionAlgorithm.Random:
                        focusedSkill = filtered[Random.Range(0, filtered.Count)];
                        break;
                }

                if (skillGroup.uniqueNodes)
                {
                    filtered.Remove(focusedSkill);
                }

                // Add to graph
                SkillNode focusedNode = new SkillNode(graph.GetAvailableNodeID(), focusedSkill, focusPoint, new());
                graph.skillNodes.Add(focusedNode);

                // Add to potential starter if possible
                if (settings.possibleStartingSkillSizes.Contains(focusedNode.skill.size))
                {
                    possibleStarterNodes.Add(focusedNode);
                }

                // Randomly decide how many connections the focused node should have
                int focusedConnectionCount = Random.Range(1, settings.maxPossibleConnectionsPerSkill + 1);

                List<SkillNode> groupSkillNodes = new() { focusedNode };
                List<SkillNode> alreadyFocused = new() { focusedNode };
                bool smallToLarge = settings.connectionAlgorithm == WebGenerationSettings.ConnectionAlgorithm.SmallToLarge;

                // Add nodes to the graph equal to the skill group count
                for (int i = 0; i < skillGroup.count - 1 && filtered.Count > 0; i++)
                {
                    // If connections of focused have maxed out, set a random skill that hasn't already been focused as the focus
                    if (focusedNode.connections.Count >= focusedConnectionCount)
                    {
                        var notFocused = groupSkillNodes.Where(x => !alreadyFocused.Contains(x)).ToList();
                        focusedNode = notFocused[Random.Range(0, notFocused.Count)];
                        alreadyFocused.Add(focusedNode);
                        focusedConnectionCount = Random.Range(1, settings.maxPossibleConnectionsPerSkill + 1);
                    }

                    // Get the current smallest and largest size in the filtered list
                    var smallest = filtered.OrderBy(x => x.size).FirstOrDefault().size;
                    var largest = filtered.OrderByDescending(x => x.size).FirstOrDefault().size;

                    // Randomly select a skill based on the connection algorithm type
                    Skill skill;

                    Skill.Size size;
                    bool toggle;

                    switch (settings.connectionAlgorithm)
                    {
                        case WebGenerationSettings.ConnectionAlgorithm.SmallToLarge:
                            if (smallToLarge)
                            {
                                size = focusedNode.skill.size.Next();
                            }
                            else
                            {
                                size = focusedNode.skill.size.Previous();
                            }

                            skill = filtered.SelectRandomBySize(size);

                            toggle = skill.size == largest;

                            if (toggle)
                            {
                                smallToLarge = !smallToLarge;
                            }
                            break;

                        case WebGenerationSettings.ConnectionAlgorithm.LargeToSmall:
                            if (smallToLarge)
                            {
                                size = focusedNode.skill.size.Next();

                            }
                            else
                            {
                                size = focusedNode.skill.size.Previous();
                            }

                            skill = filtered.SelectRandomBySize(size);

                            toggle = skill.size == smallest;

                            if (toggle)
                            {
                                smallToLarge = !smallToLarge;
                            }
                            break;

                        default:
                        case WebGenerationSettings.ConnectionAlgorithm.Random:
                            skill = filtered[Random.Range(0, filtered.Count)];
                            break;
                    }

                    if (skillGroup.uniqueNodes)
                    {
                        filtered.Remove(skill);
                    }

                    // Get position around focused
                    var position = GenerateNodePosition(skillGroup.positionSearch, focusedNode.position, skillGroup.minPreferredDistance, skillGroup.maxPreferredDistance, usedPositions, existingConnections);

                    // Create the skill node
                    var skillNode = new SkillNode(graph.GetAvailableNodeID(), skill, position, new());

                    // Add to potential starter if possible
                    if (settings.possibleStartingSkillSizes.Contains(skillNode.skill.size))
                    {
                        possibleStarterNodes.Add(skillNode);
                    }

                    // Get style index
                    int styleIndex = 0;

                    switch (skillNode.skill.size)
                    {
                        case Skill.Size.Tiny:
                            styleIndex = settings.connectionToTinyStyleIndex;
                            break;

                        case Skill.Size.Small:
                            styleIndex = settings.connectionToSmallStyleIndex;
                            break;

                        case Skill.Size.Medium:
                            styleIndex = settings.connectionToMediumStyleIndex;
                            break;

                        case Skill.Size.Large:
                            styleIndex = settings.connectionToLargeStyleIndex;
                            break;

                        case Skill.Size.Giant:
                            styleIndex = settings.connectionToGiantStyleIndex;
                            break;
                    }

                    // Create connection from focused
                    var connection = new Connection(focusedNode.id, skillNode.id, 0, 0, styleIndex);
                    Vector2[] lineSegment = new Vector2[2] { focusedNode.position, skillNode.position };
                    existingConnections.Add(lineSegment);
                    skillNode.connections.Add(connection);
                    focusedNode.connections.Add(connection);

                    groupSkillNodes.Add(skillNode);
                    graph.skillNodes.Add(skillNode);
                    graph.connections.Add(connection);
                    usedPositions.Add(position);
                }
            }

            // Randomly select starters
            int starterCount = Random.Range(settings.minStartingSkills, settings.maxStartingSkills + 1);

            for (int i = 0; i < starterCount; i++)
            {
                possibleStarterNodes[Random.Range(0, possibleStarterNodes.Count)].hasConnectionDependency = false;
            }

            return graph;
        }

        /// <summary>
        /// Selects a random skill closest to the target size.
        /// </summary>
        /// <param name="skills">The skill list.</param>
        /// <param name="targetSize">The target size.</param>
        /// <returns>A random skill from the list closest to the target size.</returns>
        public static Skill SelectRandomBySize(this List<Skill> skills, Skill.Size targetSize)
        {
            // Get minimal ordinal distance
            int minDistance = skills.Min(x => Mathf.Abs((int)x.size - (int)targetSize));

            // Filter those with matching closest distance
            var closest = skills.Where(x => Mathf.Abs((int)x.size - (int)targetSize) == minDistance).ToList();

            // Pick one randomly
            return closest[Random.Range(0, closest.Count)];
        }

        /// <summary>
        /// Generates a node position.
        /// </summary>
        /// <param name="positionSearch">The position search settings.</param>
        /// <param name="startingPoint">The starting point of the search.</param>
        /// <param name="minDistance">Minimum distance from the starting point.</param>
        /// <param name="maxDistance">Maximum distance from the starting point.</param>
        /// <param name="usedPositions">A list of used positions.</param>
        /// <param name="existingConnections">A list of existing connections.</param>
        /// <returns>A generated node position.</returns>
        public static Vector2 GenerateNodePosition(WebGenerationSettings.PositionSearch positionSearch, Vector2 startingPoint, float minDistance, float maxDistance, List<Vector2> usedPositions = null, List<Vector2[]> existingConnections = null)
        {
            var defaultSearch = WebGenerationSettings.PositionSearch.Default;

            if (positionSearch.angleStep == 0)
            {
                positionSearch.angleStep = defaultSearch.angleStep;
            }

            if (positionSearch.spiralGrowth == 0)
            {
                positionSearch.spiralGrowth = defaultSearch.spiralGrowth;
            }

            if (positionSearch.angleStepRange == Vector2.zero)
            {
                positionSearch.angleStepRange = defaultSearch.angleStepRange;
            }

            if (positionSearch.spiralGrowthRange == Vector2.zero)
            {
                positionSearch.spiralGrowthRange = defaultSearch.spiralGrowthRange;
            }

            usedPositions ??= new List<Vector2>();
            existingConnections ??= new List<Vector2[]>();

            float fallbackRadiusIncrease = minDistance * 0.2f;

            float spiralGrowth = positionSearch.GetSpiralGrowth();
            float angleStep = positionSearch.GetAngleStep();

            Vector2 lastAttempted = startingPoint;

            float maxRadius = maxDistance * 2;
            float radius = minDistance;

            for (float angleDeg = 0f; radius <= maxDistance; angleDeg += angleStep)
            {
                float angleRad = angleDeg * Mathf.Deg2Rad;
                radius = minDistance + (angleDeg / 360f) * spiralGrowth;

                if (radius >= maxDistance)
                {
                    maxDistance += fallbackRadiusIncrease;
                }

                Vector2 candidate = startingPoint + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
                lastAttempted = candidate;

                bool tooClose = usedPositions.Any(p => Vector2.Distance(p, candidate) < minDistance);

                if (!tooClose)
                {
                    return candidate;
                }
            }

            Debug.LogWarning("Fallback: Returning last attempted node due to tight space.");
            return lastAttempted;
        }

        /// <summary>
        /// Gets the shortest distance from a point to a line segment.
        /// </summary>
        public static float DistanceFromPointToLineSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            Vector2 ap = p - a;
            float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
            Vector2 projection = a + t * ab;
            return Vector2.Distance(p, projection);
        }

        public static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            float o1 = Orientation(p1, p2, q1);
            float o2 = Orientation(p1, p2, q2);
            float o3 = Orientation(q1, q2, p1);
            float o4 = Orientation(q1, q2, p2);

            if (o1 != o2 && o3 != o4) return true;

            return OnSegment(p1, q1, p2) || OnSegment(p1, q2, p2) ||
                   OnSegment(q1, p1, q2) || OnSegment(q1, p2, q2);
        }

        private static float Orientation(Vector2 a, Vector2 b, Vector2 c)
        {
            float val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (Mathf.Approximately(val, 0)) return 0;
            return val > 0 ? 1 : 2;
        }

        private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            return q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                   q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y);
        }
    }
}