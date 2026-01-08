//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Esper.SkillWeb.Graph
{
    /// <summary>
    /// The runtime version of the WebGraph.
    /// </summary>
    public class Web
    {
        /// <summary>
        /// The graph reference. This is used to create the actual runtime graph. This is either a graph created
        /// through the Web Creator, or a procedurally generated graph.
        /// </summary>
        public WebGraph graph;

        /// <summary>
        /// A dictionary of skill nodes.
        /// </summary>
        public Dictionary<int, SkillNode> skillNodes = new();

        /// <summary>
        /// The revertable graph state simply as a dictionary where the key is the skill node ID and the value is
        /// the skill node level.
        /// </summary>
        protected Dictionary<int, int> revertableState = new();

        /// <summary>
        /// The dataset.
        /// </summary>
        public WebDataset dataset;

        /// <summary>
        /// A list of all skill connections.
        /// </summary>
        public List<Connection> Connections { get => graph.connections; }

        /// <summary>
        /// The skill points set when the graph is reverted (int).
        /// </summary>
        protected int revertableSkillPointsInt;

        /// <summary>
        /// The skill points set when the graph is reverted (float).
        /// </summary>
        protected float revertableSkillPointsFloat;

        /// <summary>
        /// The skill points set when the graph is reverted (double).
        /// </summary>
        protected double revertableSkillPointsDouble;

        /// <summary>
        /// If the graph is currently reverting. This flag is automatically set as needed when RevertChanges is called.
        /// </summary>
        public bool isReverting;

        /// <summary>
        /// If there are unsaved changes made to the graph.
        /// </summary>
        public bool HasUnsavedChanges { get => revertableState.Count != 0; }

        /// <summary>
        /// Used for skill point binding (int).
        /// </summary>
        protected BoundSkillPoints<int> boundInt;

        /// <summary>
        /// Used for skill point binding (float).
        /// </summary>
        protected BoundSkillPoints<float> boundFloat;

        /// <summary>
        /// Used for skill point binding (double).
        /// </summary>
        protected BoundSkillPoints<double> boundDouble;

        /// <summary>
        /// A callback for when all skills of a web is reset. This accepts 1 argument: the web that was reset (Web).
        /// </summary>
        public static UnityEvent<Web> onSkillsReset = new();

        /// <summary>
        /// A callback for when a web is reverted. This accepts 1 argument: the reverted web (Web).
        /// </summary>
        public static UnityEvent<Web> onReverted = new();

        /// <summary>
        /// A callback for when a dangling levels are detected. This accepts 2 arguments: the affected web (Web), total number of dangling skill levels (int).
        /// A dangling level occurs when a skill web has changed in a way that no longer aligns with saved data (for example, a 
        /// skill being removed or restructured), leaving behind level values that cannot be mapped to any valid skill. This 
        /// event tracks how many such levels remain unresolved. Dangling levels are lost once the affected save data is 
        /// overwritten.
        /// </summary>
        public static UnityEvent<Web, int> onDanglingLevelsDetected = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The web graph reference.</param>
        public Web(WebGraph graph)
        {
            this.graph = graph;

            skillNodes.Clear();

            // Reset skill levels and ensure connections are updated
            foreach (var skillNode in graph.skillNodes)
            {
                var copy = skillNode.CreateCopy();
                copy.web = this;
                copy.uiRefresher = null;
                copy.uiOnStateChanged = null;
                copy.SetLevel(0, false);
                copy.state = Skill.State.Locked;
                copy.connections = GetSkillConnections(copy);
                skillNodes.Add(copy.id, copy);
            }

            // Default binding
            boundInt = new BoundSkillPoints<int>()
            {
                skillPointsGetter = () => SkillWeb.skillPoints,
                skillCostGetter = skillNode => 1,
                skillPointsSetter = x => SkillWeb.skillPoints = x
            };

            if (graph.dataset)
            {
                dataset = UnityEngine.Object.Instantiate(graph.dataset);
            }

            UpdateAllStates();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="savableWeb">The savable web.</param>
        public Web(SavableWeb savableWeb)
        {
            // Create a list of skill nodes
            List<SkillNode> skillNodes = new();

            foreach (var savableNode in savableWeb.savableSkillNodes)
            {
                var skillNode = savableNode.ToSkillNode();
                skillNode.webGraphID = savableWeb.id;
                skillNode.web = this;
                skillNodes.Add(skillNode);
            }

            // Get the existing graph if it exists
            graph = SkillWeb.GetWebGraph(savableWeb.id);

            bool changeDetected = false;

            if (graph)
            {
                // Detect a graph change
                changeDetected = savableWeb.changeGuid != graph.changeGuid;

                if (changeDetected)
                {
                    SkillWebLogger.Log("Skill Web: Web change detected. Attempting to restore data...");
                    string message = string.Empty;

                    // Set skill nodes to new nodes from the changed graph
                    skillNodes.Clear();

                    foreach (var skillNode in graph.skillNodes)
                    {
                        var copy = skillNode.CreateCopy();
                        copy.web = this;

                        // Compare GUIDs from saved web and retrieve any skill node data where possible
                        if (!string.IsNullOrEmpty(copy.guid))
                        {
                            foreach (var savedNode in savableWeb.savableSkillNodes)
                            {
                                if (string.IsNullOrEmpty(savedNode.guid))
                                {
                                    continue;
                                }

                                if (copy.guid == savedNode.guid)
                                {
                                    if (copy.Level != savedNode.level)
                                    {
                                        copy.SetLevel(savedNode.level, false);  
                                        message += $"Skill level restored for {copy.skill.skillName}.\n";
                                    }
                                }
                            }
                        }
                        // If guid is empty, sse backup restoration by comparing IDs
                        else
                        {
                            foreach (var savedNode in savableWeb.savableSkillNodes)
                            {
                                Vector2 savedPos = new Vector2(savedNode.position[0], savedNode.position[1]);

                                if (copy.id == savedNode.id && copy.skill.id == savedNode.skillID)
                                {
                                    if (copy.Level != savedNode.level)
                                    {
                                        copy.SetLevel(savedNode.level, false);
                                        message += $"Skill level restored for {copy.skill.skillName}.\n";
                                    }
                                }
                            }
                        }

                        skillNodes.Add(copy);
                    }

                    savableWeb.changeGuid = graph.changeGuid;

                    if (string.IsNullOrEmpty(message))
                    {
                        message = "Failed to restore any skill data.";
                    }
                    else
                    {
                        message += "No additional data needed restoration, or the remaining data could not be restored.";
                    }

                    SkillWebLogger.Log(message);
                }

                if (graph.dataset)
                {
                    dataset = UnityEngine.Object.Instantiate(graph.dataset);
                }
            }
            else
            {
                // Recreate the generated graph
                graph = ScriptableObject.CreateInstance<WebGraph>();
                graph.id = savableWeb.id;
                graph.GenerateDataset();
                graph.skillNodes = skillNodes;
                graph.connections = savableWeb.connections;
            }

            // Create skill node dictionary
            this.skillNodes.Clear();

            foreach (var skillNode in skillNodes)
            {
                skillNode.connections = GetSkillConnections(skillNode);
                this.skillNodes.Add(skillNode.id, skillNode);
            }

            // Default binding
            boundInt = new BoundSkillPoints<int>()
            {
                skillPointsGetter = () => SkillWeb.skillPoints,
                skillCostGetter = skillNode => 1,
                skillPointsSetter = x => SkillWeb.skillPoints = x
            };

            UpdateAllStates();

            if (changeDetected)
            {
                int totalLevels = SkillLevelTotal();
                int savableTotalLevels = savableWeb.SkillLevelTotal();

                if (savableTotalLevels > totalLevels)
                {
                    int danglingLevels = savableTotalLevels - totalLevels;
                    onDanglingLevelsDetected.Invoke(this, danglingLevels);
                }
            }
        }

        /// <summary>
        /// Gets the sum of the level of all skills.
        /// </summary>
        /// <returns>The total combined level of all skills.</returns>
        public int SkillLevelTotal()
        {
            int total = 0;

            foreach (var skill in skillNodes.Values)
            {
                total += skill.Level;
            }

            return total;
        }

        /// <summary>
        /// Resets all skills.
        /// </summary>
        public void ResetAllSkills()
        {
            foreach (var skillNode in skillNodes.Values)
            {
                if (skillNode.Level > 0)
                {
                    skillNode.ResetLevel();
                }
            }
        }

        /// <summary>
        /// Adds to the revertable state. If this is the first skill node added, the web will become revertable. The web is
        /// only revertable if changes have been made.
        /// </summary>
        public virtual void AddToRevertableState(SkillNode skillNode)
        {
            if (revertableState.ContainsKey(skillNode.id))
            {
                return;
            }

            if (revertableState.Count == 0)
            {
                if (boundInt != null)
                {
                    revertableSkillPointsInt = boundInt.skillPointsGetter();
                }
                else if (boundFloat != null)
                {
                    revertableSkillPointsFloat = boundFloat.skillPointsGetter();
                }
                else if (boundDouble != null)
                {
                    revertableSkillPointsDouble = boundDouble.skillPointsGetter();
                }
            }

            revertableState.Add(skillNode.id, skillNode.Level);
        }

        /// <summary>
        /// Confirms the changes made. The graph will not be revertable until changes are made once again (SetCurrentStateRevertable is called).
        /// </summary>
        public virtual void ConfirmChanges()
        {
            revertableState.Clear();
        }

        /// <summary>
        /// Reverts the changes made. The graph will not be revertable until changes are made once again (SetCurrentStateRevertable is called).
        /// </summary>
        public virtual void RevertChanges()
        {
            if (HasUnsavedChanges)
            {
                isReverting = true;

                foreach (var state in revertableState)
                {
                    skillNodes[state.Key].SetLevel(state.Value);
                }

                isReverting = false;

                revertableState.Clear();

                if (boundInt != null)
                {
                    boundInt.skillPointsSetter(revertableSkillPointsInt);
                }
                else if (boundFloat != null)
                {
                    boundFloat.skillPointsSetter(revertableSkillPointsFloat);
                }
                else if (boundDouble != null)
                {
                    boundDouble.skillPointsSetter(revertableSkillPointsDouble);
                }

                onReverted.Invoke(this);
            }
            else
            {
                SkillWebLogger.LogWarning("Web View: cannot revert when no changes were made.");
            }
        }

        /// <summary>
        /// Updates all node states.
        /// </summary>
        public void UpdateAllStates()
        {
            foreach (var skillNode in skillNodes.Values)
            {
                skillNode.UpdateState();
            }
        }

        /// <summary>
        /// Creates a list of connections of the skill node.
        /// </summary>
        /// <param name="skillNode">The skill node.</param>
        /// <returns>A list of connections of the skill node.</returns>
        public List<Connection> GetSkillConnections(SkillNode skillNode)
        {
            List<Connection> connections = new();

            foreach (var connection in graph.connections)
            {
                if (skillNode.id == connection.inputNodeID || skillNode.id == connection.outputNodeID)
                {
                    connections.Add(connection);
                }
            }

            return connections;
        }

        /// <summary>
        /// Gets a skill node by its node ID. This is not the skill ID.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <returns>The skill node with the specific ID or null if it doesn't exist.</returns>
        public SkillNode GetNode(int id)
        {
            if (!skillNodes.ContainsKey(id))
            {
                SkillWebLogger.LogWarning($"Skill Web: the skill node with the ID {id} does not exist! Has the web been initialized?");
                return null;
            }

            return skillNodes[id];
        }

        /// <summary>
        /// Gets skill nodes by their skill ID. A list is returned because multiple instances of the same skill are allowed
        /// in a web. 
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>A list of skills with a matching skill ID. The list will be empty if none with a matching ID were found.</returns>
        public List<SkillNode> GetSkillNodes(int id)
        {
            List<SkillNode> matching = new();

            foreach (var skillNode in skillNodes.Values)
            {
                if (skillNode.skill.id == id)
                {
                    matching.Add(skillNode);
                }
            }

            return matching;
        }

        /// <summary>
        /// Gets skill nodes by their name. A list is returned because multiple instances of the same skill are allowed
        /// in a web. 
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>A list of skills with a matching name. The list will be empty if none with a matching name were found.</returns>
        public List<SkillNode> GetSkillNodes(string name)
        {
            List<SkillNode> matching = new();

            foreach (var skillNode in skillNodes.Values)
            {
                if (skillNode.skill.skillName == name)
                {
                    matching.Add(skillNode);
                }
            }

            return matching;
        }

        /// <summary>
        /// Gets the first skill node by skill ID.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>The skill node with the specific ID or null if it doesn't exist.</returns>
        public SkillNode GetSkillNode(int id)
        {
            foreach (var skillNode in skillNodes.Values)
            {
                if (skillNode.skill.id == id)
                {
                    return skillNode;
                }
            }

            SkillWebLogger.LogWarning($"Skill Web: the skill node with the ID {id} does not exist in the web! Has the web been initialized?");
            return null;
        }

        /// <summary>
        /// Gets the first skill node by name.
        /// </summary>
        /// <param name="name">The name of the skill.</param>
        /// <returns>The skill node with the specific name or null if it doesn't exist.</returns>
        public SkillNode GetSkillNode(string name)
        {
            foreach (var skillNode in skillNodes.Values)
            {
                if (skillNode.skill.skillName == name)
                {
                    return skillNode;
                }
            }

            SkillWebLogger.LogWarning($"Skill Web: the skill node with the name {name} does not exist in the web! Has the web been initialized?");
            return null;
        }

        /// <summary>
        /// Gets a list of all skill nodes in specific states.
        /// </summary>
        /// <param name="states">The skill states.</param>
        /// <returns>A list of skill nodes in specific states.</returns>
        public List<SkillNode> GetSkillNodes(params Skill.State[] states)
        {
            List<SkillNode> skillNodes = new();

            foreach (var skillNode in this.skillNodes.Values)
            {
                if (states.Contains(skillNode.state))
                {
                    skillNodes.Add(skillNode);
                }
            }

            return skillNodes;
        }

        /// <summary>
        /// Gets a list of all obtained skill nodes.
        /// </summary>
        /// <returns>A list of all obtained skill nodes.</returns>
        public List<SkillNode> GetObtainedSkillNodes()
        {
            return GetSkillNodes(Skill.State.Obtained, Skill.State.Maxed);
        }

        /// <summary>
        /// Checks if a skill node with the specific node ID is obtained.
        /// </summary>
        /// <param name="id">The node ID. This is not the skill ID.</param>
        /// <returns>True if the skill node with the ID is obtained. Otherwise, false.</returns>
        public bool IsNodeObtained(int id)
        {
            var node = GetNode(id);

            if (node == null)
            {
                return false;
            }

            return node.IsObtained;
        }

        /// <summary>
        /// Checks if a skill with the specific skill ID is obtained.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>True if the skill node with the skill ID is obtained. Otherwise, false.</returns>
        public bool IsObtained(int id)
        {
            var skill = GetSkillNode(id);

            if (skill == null)
            {
                return false;
            }

            return skill.IsObtained;
        }

        /// <summary>
        /// Checks if a skill with the specific skill name is obtained.
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>True if the skill node with the name is obtained. Otherwise, false.</returns>
        public bool IsObtained(string name)
        {
            var skill = GetSkillNode(name);

            if (skill == null)
            {
                return false;
            }

            return skill.IsObtained;
        }

        /// <summary>
        /// Converts this Web to a SavableWeb.
        /// </summary>
        /// <returns>The Web converted to a savable version.</returns>
        public SavableWeb ToSavable()
        {
            List<SavableSkillNode> savableSkillNodes = new();

            foreach (var skillNode in skillNodes.Values)
            {
                savableSkillNodes.Add(skillNode.ToSavable());
            }

            var savableWeb = new SavableWeb()
            {
                id = graph.id,
                changeGuid = graph.changeGuid,
                savableSkillNodes = savableSkillNodes,
                connections = Connections
            };

            return savableWeb;
        }

        /// <summary>
        /// Checks if the player has enough skill points to upgrade the skill node.
        /// </summary>
        /// <param name="skillNode">The skill node.</param>
        /// <returns>True if the player has enough skill points. Otherwise, false.</returns>
        public bool CanUpgradeSkillNode(SkillNode skillNode)
        {
            if (boundInt != null)
            {
                return boundInt.skillPointsGetter() >= boundInt.skillCostGetter(skillNode);
            }
            else if (boundFloat != null)
            {
                return boundFloat.skillPointsGetter() >= boundFloat.skillCostGetter(skillNode);
            }
            else if (boundDouble != null)
            {
                return boundDouble.skillPointsGetter() >= boundDouble.skillCostGetter(skillNode);
            }

            return true;
        }

        /// <summary>
        /// Applies the skill points binding while using a skill node reference as if it was reset.
        /// </summary>
        /// <param name="skillNode">The skill node.</param>
        public void ApplyBindingReset(SkillNode skillNode)
        {
            if (boundInt != null)
            {
                boundInt.skillPointsSetter(boundInt.skillPointsGetter() + boundInt.skillCostGetter(skillNode) * skillNode.Level);
            }
            else if (boundFloat != null)
            {
                boundFloat.skillPointsSetter(boundFloat.skillPointsGetter() + boundFloat.skillCostGetter(skillNode) * skillNode.Level);
            }
            else if (boundDouble != null)
            {
                boundDouble.skillPointsSetter(boundDouble.skillPointsGetter() + boundDouble.skillCostGetter(skillNode) * skillNode.Level);
            }
        }

        /// <summary>
        /// Applies the skill points binding.
        /// </summary>
        /// <param name="skillNode">The skill node.</param>
        /// <param name="upgraded">If the skill was upgraded.</param>
        public void ApplyBinding(SkillNode skillNode, bool upgraded)
        {
            if (boundInt != null)
            {
                if (upgraded)
                {
                    var skillPoints = boundInt.skillPointsGetter();
                    var cost = boundInt.skillCostGetter(skillNode);

                    if (skillPoints < cost)
                    {
                        cost = skillPoints;
                    }

                    boundInt.skillPointsSetter(skillPoints - cost);
                }
                else
                {
                    boundInt.skillPointsSetter(boundInt.skillPointsGetter() + boundInt.skillCostGetter(skillNode));
                }
            }
            else if (boundFloat != null)
            {
                if (upgraded)
                {
                    var skillPoints = boundFloat.skillPointsGetter();
                    var cost = boundFloat.skillCostGetter(skillNode);

                    if (skillPoints < cost)
                    {
                        cost = skillPoints;
                    }

                    boundFloat.skillPointsSetter(skillPoints - cost);
                }
                else
                {
                    boundFloat.skillPointsSetter(boundFloat.skillPointsGetter() + boundFloat.skillCostGetter(skillNode));
                }
            }
            else if (boundDouble != null)
            {
                if (upgraded)
                {
                    var skillPoints = boundDouble.skillPointsGetter();
                    var cost = boundDouble.skillCostGetter(skillNode);

                    if (skillPoints < cost)
                    {
                        cost = skillPoints;
                    }

                    boundDouble.skillPointsSetter(skillPoints - cost);
                }
                else
                {
                    boundDouble.skillPointsSetter(boundDouble.skillPointsGetter() + boundDouble.skillCostGetter(skillNode));
                }
            }
        }

        /// <summary>
        /// Binds this web with a skill points value.
        /// </summary>
        /// <param name="skillPointsGetter">A method that gets the player's skill points. This should return an int.</param>
        /// <param name="skillCostGetter">A method that gets the skill cost from a skill node.</param>
        /// <param name="skillPointsSetter">A method that sets the player's skill points after applying the cost. The cost 
        /// will be subtracted if the skill node was upgraded. If it was downgraded, it will be added. The value will not be 
        /// set to a number less than 0.</param>
        public void Bind(Func<int> skillPointsGetter, Func<SkillNode, int> skillCostGetter, Action<int> skillPointsSetter)
        {
            boundFloat = null;
            boundDouble = null;
             
            boundInt = new BoundSkillPoints<int>()
            {
                skillPointsGetter = skillPointsGetter,
                skillCostGetter = skillCostGetter,
                skillPointsSetter = skillPointsSetter
            };
        }

        /// <summary>
        /// Binds this web with a skill points value.
        /// </summary>
        /// <param name="skillPointsGetter">A method that gets the player's skill points. This should return a float.</param>
        /// <param name="skillCostGetter">A method that gets the skill cost from a skill node.</param>
        /// <param name="skillPointsSetter">A method that sets the player's skill points after applying the cost. The cost 
        /// will be subtracted if the skill node was upgraded. If it was downgraded, it will be added. The value will not be 
        /// set to a number less than 0.</param>
        public void Bind(Func<float> skillPointsGetter, Func<SkillNode, float> skillCostGetter, Action<float> skillPointsSetter)
        {
            boundInt = null;
            boundDouble = null;

            boundFloat = new BoundSkillPoints<float>()
            {
                skillPointsGetter = skillPointsGetter,
                skillCostGetter = skillCostGetter,
                skillPointsSetter = skillPointsSetter
            };
        }

        /// <summary>
        /// Binds this web with a skill points value.
        /// </summary>
        /// <param name="skillPointsGetter">A method that gets the player's skill points. This should return a double.</param>
        /// <param name="skillCostGetter">A method that gets the skill cost from a skill node.</param>
        /// <param name="skillPointsSetter">A method that sets the player's skill points after applying the cost. The cost 
        /// will be subtracted if the skill node was upgraded. If it was downgraded, it will be added. The value will not be 
        /// set to a number less than 0.</param>
        public void Bind(Func<double> skillPointsGetter, Func<SkillNode, double> skillCostGetter, Action<double> skillPointsSetter)
        {
            boundInt = null;
            boundFloat = null;

            boundDouble = new BoundSkillPoints<double>()
            {
                skillPointsGetter = skillPointsGetter,
                skillCostGetter = skillCostGetter,
                skillPointsSetter = skillPointsSetter
            };
        }

        /// <summary>
        /// Skill points binder.
        /// </summary>
        public class BoundSkillPoints<T>
        {
            /// <summary>
            /// A method that gets the player's skill points. This should return the numeric type.
            /// </summary>
            public Func<T> skillPointsGetter;

            /// <summary>
            /// A method that gets the skill cost from a skill node.
            /// </summary>
            public Func<SkillNode, T> skillCostGetter;

            /// <summary>
            /// A method that sets the player's skill points after applying the cost. The cost will be subtracted if the
            /// skill node was upgraded. If it was downgraded, it will be added. The value will not be set to a number
            /// less than 0.
            /// </summary>
            public Action<T> skillPointsSetter;
        }
    }
}