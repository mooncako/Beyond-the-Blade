//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Attributes;
using Esper.SkillWeb.DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Esper.SkillWeb.Graph
{
    /// <summary>
    /// A node that represents a skill.
    /// </summary>
    [Serializable]
    public class SkillNode : Node
    {
        /// <summary>
        /// The web that this skill is a part of.
        /// </summary>
        public Web web;

        /// <summary>
        /// The skill.
        /// </summary>
        public Skill skill;

        /// <summary>
        /// The current state of the skill node.
        /// </summary>
        [NonSerialized]
        public Skill.State state;

        /// <summary>
        /// Dictates whether this skill should become unlockable only if it has a direct connection to another skill
        /// that is obtained.
        /// </summary>
        public bool hasConnectionDependency = true;

        /// <summary>
        /// The number of direct connections to obtained skills required for this skill to be unlockable.
        /// </summary>
        public int dependencyCount = 1;

        /// <summary>
        /// The number of direct connections to maxed skills required for this skill to be unlockable. This should 
        /// always be equal to or smaller than the dependencyCount.
        /// </summary>
        public int maxedRequirementCount = 0;

        /// <summary>
        /// An action that refreshes the UI. This is invoked when the skill state is updated.
        /// </summary>
        [NonSerialized]
        public Action uiRefresher;

        /// <summary>
        /// An action that is simply meant to invoke the onStateChanged event on the UI side.
        /// </summary>
        [NonSerialized]
        public Action uiOnStateChanged;

        /// <summary>
        /// The dataset. This is cloned from the Skill reference.
        /// </summary>
        public SkillDataset dataset;

#if UNITY_EDITOR
        /// <summary>
        /// This controls the hidden state of the fields in the Web Creator. This is only available in the Unity Editor.
        /// </summary>
        public bool hideFieldsInWebGraphEditor;
#endif

        /// <summary>
        /// The current skill level.
        /// </summary>
        public int Level { get; protected set; }

        /// <summary>
        /// The max skill level.
        /// </summary>
        public int MaxLevel { get => skill.maxLevel; }

        /// <summary>
        /// If the skill node is locked. 
        /// </summary>
        public bool IsLocked { get => state == Skill.State.Locked; }

        /// <summary>
        /// If the skill is unlocked.
        /// </summary>
        public bool IsUnlocked { get => state != Skill.State.Locked; }

        /// <summary>
        /// If the skill has been obtained (leveled up atleast once).
        /// </summary>
        public bool IsObtained { get => state == Skill.State.Obtained || state == Skill.State.Maxed; }

        /// <summary>
        /// If the skill has been maxed.
        /// </summary>
        public bool IsMaxed { get => state == Skill.State.Maxed; }

        /// <summary>
        /// If the skill can be upgraded (unlocked and not maxed).
        /// </summary>
        public bool IsUpgradable { get => IsUnlocked && !IsMaxed; }

        /// <summary>
        /// A function that gets the player level. You can use this to set your own level getting logic. By default,
        /// it just returns the SkillWeb.playerLevel field.
        /// </summary>
        public static Func<int> playerLevelGetter = () => SkillWeb.playerLevel;

        /// <summary>
        /// A function that checks if a skill can be upgraded. You can use this to set your own logic to prevent/allow
        /// skill upgrades.
        /// </summary>
        public static Func<SkillNode, bool> canUpgrade = (skill) => true;

        /// <summary>
        /// A function that checks if a skill can be downgraded. You can use this to set your own logic to prevent/allow 
        /// skill downgrades.
        /// </summary>
        public static Func<SkillNode, bool> canDowngrade = (skill) => true;

        /// <summary>
        /// A callback for when any skill is upgraded. This accepts 1 argument: the affected skill (SkillNode).
        /// </summary>
        public static UnityEvent<SkillNode> onUpgrade = new();

        /// <summary>
        /// A callback for when any skill is downgraded. This accepts 1 argument: the affected skill (SkillNode).
        /// </summary>
        public static UnityEvent<SkillNode> onDowngrade = new();

        /// <summary>
        /// A callback for when any skill is obtained. This accepts 1 argument: the affected skill (SkillNode).
        /// </summary>
        public static UnityEvent<SkillNode> onSkillObtained = new();

        /// <summary>
        /// A callback for when any skill becomes unobtained. This accepts 1 argument: the affected skill (SkillNode).
        /// </summary>
        public static UnityEvent<SkillNode> onSkillUnobtained = new();

        /// <summary>
        /// A callback for when any skill state is changed. This accepts 1 argument: the affected skill (SkillNode).
        /// </summary>
        public static UnityEvent<SkillNode> onSkillStateChanged = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="position">The position in the graph.</param>
        /// <param name="connections">A list of all connections.</param>
        public SkillNode(int id, Skill skill, Vector2 position, List<Connection> connections = null) : base(id, position, connections)
        {
            this.skill = skill;

            if (skill && skill.dataset)
            {
                dataset = UnityEngine.Object.Instantiate(skill.dataset);
            }
        }

        /// <summary>
        /// Creates a copy of this skill node.
        /// </summary>
        /// <returns>The copy.</returns>
        public SkillNode CreateCopy()
        {
            var copy = new SkillNode(id, skill, position, connections);
            copy.guid = guid;
#if UNITY_EDITOR
            copy.hideFieldsInWebGraphEditor = hideFieldsInWebGraphEditor;
#endif
            copy.hasConnectionDependency = hasConnectionDependency;
            copy.dependencyCount = dependencyCount;
            copy.maxedRequirementCount = maxedRequirementCount;
            copy.webGraphID = webGraphID;
            return copy;
        }

        /// <summary>
        /// Gets the skill icon based on the current skill state.
        /// </summary>
        /// <param name="useFallback">If the requested icon isn't set, a fallback should be returned.</param>
        /// <returns>The SkillIcon.</returns>
        public Skill.SkillIcon GetIcon(bool useFallback)
        {
            return skill.GetIcon(state, useFallback);
        }

        /// <summary>
        /// Sets the skill level. The level cannot be higher than the max level.
        /// </summary>
        /// <param name="level">The new level.</param>
        /// <param name="updateState">If the state should be updated.</param>
        public void SetLevel(int level, bool updateState = true)
        {
            if (level > MaxLevel || level < 0)
            {
                return;
            }

            Level = level;
            ApplyCustomScaling();

            if (updateState)
            {
                if (web.isReverting)
                {
                    UpdateState(true);
                }
                else
                {
                    UpdateState();         
                }

                UpdateConnectedSkillStates();
            }
        }

        /// <summary>
        /// Resets the level of this skill to 0.
        /// </summary>
        /// <param name="updateState">If the state should be updated.</param>
        public void ResetLevel(bool updateState = true)
        {
            web.AddToRevertableState(this);

            // Decrement level (this way it applies cost scaling if set)
            var level = Level;
            for (int i = 0; i < level; i++)
            {
                Level--;
                ApplyCustomScaling();
                web.ApplyBinding(this, false);
            }

            SetLevel(0, updateState);
            onDowngrade?.Invoke(this);
        }

        /// <summary>
        /// Increases the level by 1 if possible. The skill must be unlocked and the level cannot be higher than the 
        /// max level.
        /// </summary>
        /// <returns>True if the skill was leveled up. Otherwise, false.</returns>
        public bool TryUpgrade()
        {
            if (IsLocked || Level >= MaxLevel || !canUpgrade(this) || !web.CanUpgradeSkillNode(this) || (SkillWeb.Settings.enablePlayerLevelRequirement && playerLevelGetter() < skill.levelRequirement))
            {
                return false;
            }

            web.AddToRevertableState(this);
            web.ApplyBinding(this, true);
            Level++;
            ApplyCustomScaling();

            UpdateState();
            UpdateConnectedSkillStates();

            onUpgrade?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Decreases the level by 1 if possible. The level cannot be lower than 0.
        /// </summary>
        /// <returns>True if the skill level was decreased. Otherwise, false.</returns>
        public bool TryDowngrade()
        {
            if (!SkillWeb.Settings.enableDowngrading || Level <= 0 || !canDowngrade(this))
            {
                return false;
            }

            web.AddToRevertableState(this);
            Level--;
            ApplyCustomScaling();
            web.ApplyBinding(this, false);

            UpdateState();
            UpdateConnectedSkillStates();

            onDowngrade?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Applies stat scaling to custom fields.
        /// </summary>
        protected void ApplyCustomScaling()
        {
            if (dataset == null)
            {
                return;
            }

            SkillScaler.ApplyScaling(dataset, Level, MaxLevel);
        }

        /// <summary>
        /// Recursively updates connected skills. This may reset skills if their preceding skill nodes have become
        /// unobtained.
        /// </summary>
        /// <param name="ignoreList">A list of connections to ignore.</param>
        public void UpdateConnectedSkillStates(List<Connection> ignoreList = null)
        {
            if (ignoreList == null)
            {
                ignoreList = new();
            }

            foreach (var connection in connections)
            {
                if (ignoreList.Contains(connection))
                {
                    continue;
                }

                ignoreList.Add(connection);
                int otherID = connection.outputNodeID != id ? connection.outputNodeID : connection.inputNodeID;
                SkillNode other;

                if (otherID == -1)
                {
                    other = null;
                }
                else
                {
                    other = web.GetNode(otherID);                  
                }

                if (other != null)
                {
                    bool wasObtained = other.IsObtained;
                    bool unlockRequirementsMet = other.UnlockRequirementsMet();
                    other.UpdateState(unlockRequirementsMet);

                    if (!unlockRequirementsMet && wasObtained)
                    {
                        other.UpdateConnectedSkillStates(ignoreList);
                    }
                }
            }
        }

        /// <summary>
        /// If this skill has a connection to a prerequisite skill. If this skill itself does not require a
        /// prerequisite, true will be returned.
        /// </summary>
        /// <param name="ignoreList">A list of connections to ignore.</param>
        /// <returns>True if there's a connection to a prerequisite skill. Otherwise, false.</returns>
        public bool HasConnectionToPrerequisiteSkill(List<Connection> ignoreList = null)
        {
            if (ignoreList == null)
            {
                ignoreList = new();
            }

            foreach (var connection in connections)
            {
                if (ignoreList.Contains(connection))
                {
                    continue;
                }

                ignoreList.Add(connection);
                int otherID = connection.outputNodeID != id ? connection.outputNodeID : connection.inputNodeID;
                SkillNode other;

                if (otherID == -1)
                {
                    other = null;
                }
                else
                {
                    other = web.GetNode(otherID);
                }

                if (other != null)
                {
                    if (other.Level == 0)
                    {
                        continue;
                    }

                    if (!other.hasConnectionDependency)
                    {
                        return true;
                    }

                    bool result = other.HasConnectionToPrerequisiteSkill(ignoreList);

                    if (result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if this skill's unlock connection requirements are met.
        /// 
        /// Requirements are met when the skill either is not dependant on connections to become unlockable OR
        /// the skill is dependant on connetions to become unlockable and this node has at least one connection
        /// to a prerequisite skill. Also the number of direct connections to obtained skills must match the 
        /// dependencyCount value. In addittion, no matter the connection state, the player level must meet the 
        /// required level.
        /// </summary>
        /// <returns>True if the unlock requirements are met. Otherwise, false.</returns>
        public bool UnlockRequirementsMet()
        {
            bool playerLevelRequirementMet = !SkillWeb.Settings.enablePlayerLevelRequirement || (SkillWeb.Settings.enablePlayerLevelRequirement && playerLevelGetter() >= skill.levelRequirement);
            bool prereqDependencyMet = !hasConnectionDependency || HasConnectionToPrerequisiteSkill();
            bool connectionDependencyMet = !hasConnectionDependency || (prereqDependencyMet && dependencyCount <= 1);
            bool maxedRequirementMet = !hasConnectionDependency || maxedRequirementCount <= 0;

            if (!connectionDependencyMet || !maxedRequirementMet)
            {
                // Count the number of valid connections (direct connections to obtained skills)
                int validDependencies = 0;
                int validMaxDependencies = 0;

                foreach (var connection in connections)
                {
                    // Get the other node
                    int otherID = connection.outputNodeID != id ? connection.outputNodeID : connection.inputNodeID;
                    SkillNode other;

                    if (otherID == -1)
                    {
                        other = null;
                    }
                    else
                    {
                        other = web.GetNode(otherID);
                    }

                    // Other is valid if obtained or maxed
                    if (other != null)
                    {
                        if (other.IsObtained)
                        {
                            validDependencies++;

                            if (other.IsMaxed)
                            {
                                validMaxDependencies++;
                            }
                        }
                    }
                }

                connectionDependencyMet = validDependencies >= dependencyCount;
                maxedRequirementMet = validMaxDependencies >= maxedRequirementCount;
            }

            return playerLevelRequirementMet && prereqDependencyMet && connectionDependencyMet && maxedRequirementMet;
        }

        /// <summary>
        /// Logically updates the skill state. This may reset skills if they're preceding skill nodes have become
        /// unobtained.
        /// </summary>
        public void UpdateState()
        {
            bool unlockRequirementsMet = UnlockRequirementsMet();
            UpdateState(unlockRequirementsMet);
        }

        /// <summary>
        /// Logically updates the skill state. This may reset skills if they're preceding skill nodes have become
        /// unobtained.
        /// </summary>
        /// <param name="unlockRequirementsMet">If the unlock requirements are met.</param>
        public void UpdateState(bool unlockRequirementsMet)
        {
            var prevState = state;

            if (!unlockRequirementsMet)
            {
                // Reset the level if unlock requirements are not met due to preceding nodes becoming unobtained
                if (Level > 0)
                {
                    ResetLevel(false);
                }

                state = Skill.State.Locked;
            }
            else
            {
                if (Level == 0)
                {
                    state = Skill.State.Unlocked;
                }
                else if (Level > 0 && Level < MaxLevel)
                {
                    state = Skill.State.Obtained;
                }
                else
                {
                    state = Skill.State.Maxed;
                }
            }

            if ((prevState == Skill.State.Locked || prevState == Skill.State.Unlocked) && IsObtained)
            {
                onSkillObtained.Invoke(this);
            }
            else if ((prevState == Skill.State.Obtained || prevState == Skill.State.Maxed) && !IsObtained)
            {
                onSkillUnobtained.Invoke(this);
            }

            if (prevState != state)
            {
                onSkillStateChanged?.Invoke(this);
                uiOnStateChanged?.Invoke();
            }

            uiRefresher?.Invoke();
        }

        /// <summary>
        /// Converts this SkillNode to a SavableSkillNode.
        /// </summary>
        /// <returns>The SkillNode converted to a savable version.</returns>
        public SavableSkillNode ToSavable()
        {
            var savableSkillNode = new SavableSkillNode()
            {
                id = id,
                guid = guid,
                skillID = skill.id,
                hasConnectionDependency = hasConnectionDependency,
                position = new float[] { position.x, position.y },
                level = Level
            };

            return savableSkillNode;
        }
    }
}