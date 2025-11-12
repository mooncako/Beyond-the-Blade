//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;
using Esper.SkillWeb.Database;
using Esper.SkillWeb.Settings;
using Esper.SkillWeb.Graph;
using System.Collections.Generic;
using System.Linq;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Simplified access to Skill Web's settings and database. Provides some other runtime functionality as well.
    /// </summary>
    public static class SkillWeb
    {
        /// <summary>
        /// The path to settings relative to the Resources folder.
        /// </summary>
        private static string settingsPath = "SkillWeb/Settings/SkillWebSettings";

        /// <summary>
        /// Skill Web's settings.
        /// </summary>
        private static SkillWebSettings settings;

        /// <summary>
        /// Skill Web's settings.
        /// </summary>
        public static SkillWebSettings Settings
        {
            get
            {
                if (!settings)
                {
                    settings = Resources.Load<SkillWebSettings>(settingsPath);
                }

                return settings;
            }
        }

        /// <summary>
        /// The current player level. Update this value whenever your player's level changes.
        /// </summary>
        public static int playerLevel;

        /// <summary>
        /// The current skill points. Update this value whenever the player gains skill points.
        /// </summary>
        public static int skillPoints;

        /// <summary>
        /// If Skill Web has been initialized.
        /// </summary>
        public static bool IsInitialized { get => SkillWebDatabase.IsConnected; }

        /// <summary>
        /// Initializes Skill Web by connecting to the database. This is required to get it to work properly at runtime.
        /// </summary>
        public static void Initialize()
        {
            SkillWebDatabase.Initialize();
        }

        /// <summary>
        /// Terminates the Skill Web connection. Initialize will need to be called again before working with Skill Web's API.
        /// This will also clear all Skill Web related events.
        /// </summary>
        public static void Terminate()
        {
            SkillWebDatabase.Disconnect();
            playerLevel = 1;
            skillPoints = 0;

            SkillNode.onUpgrade.RemoveAllListeners();
            SkillNode.onDowngrade.RemoveAllListeners();
            SkillNode.onSkillObtained.RemoveAllListeners();
            SkillNode.onSkillUnobtained.RemoveAllListeners();
            SkillNode.onSkillStateChanged.RemoveAllListeners();
            Web.onSkillsReset.RemoveAllListeners();
            Web.onReverted.RemoveAllListeners();
            Web.onDanglingLevelsDetected.RemoveAllListeners();
        }

        #region SKILLS
        /// <summary>
        /// Gets all skills.
        /// </summary>
        /// <returns>A list of all skills.</returns>
        public static Skill[] GetAllSkills()
        {
            var skills = Resources.LoadAll<Skill>(Skill.resourcesPath);
            return skills;
        }

        /// <summary>
        /// Gets all skills, split into groups based on size.
        /// </summary>
        /// <returns>A dictionary of all skills grouped by size.</returns>
        public static Dictionary<Skill.Size, List<Skill>> GetAllSkillsGroupedBySize()
        {
            var skills = Resources.LoadAll<Skill>(Skill.resourcesPath);
            Dictionary<Skill.Size, List<Skill>> dict = skills.GroupBy(skill => skill.size).ToDictionary(group => group.Key, group => group.ToList());
            return dict;
        }

        /// <summary>
        /// Gets a skill by it's ID.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>A skill with the ID or null if it does not exist.</returns>
        public static Skill GetSkill(int id)
        {
            var record = SkillWebDatabase.GetSkillRecord(id);

            if (record == null)
            {
                return null;
            }

            var skill = Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}");

            if (!skill)
            {
                SkillWebLogger.LogWarning($"Skill Web: Skill with the ID '{id}' doesn't exist.");
            }

            return skill;
        }

        /// <summary>
        /// Gets a skill by it's name.
        /// </summary>
        /// <param name="name">The name of the skill.</param>
        /// <returns>A skill with the name or null if it does not exist.</returns>
        public static Skill GetSkill(string name)
        {
            var record = SkillWebDatabase.GetSkillRecord(name);

            if (record == null)
            {
                return null;
            }

            var skill = Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}");

            if (!skill)
            {
                SkillWebLogger.LogWarning($"Skill Web: Skill with the name '{name}' doesn't exist.");
            }

            return skill;
        }

        /// <summary>
        /// Gets all skills with names that contain a given string.
        /// </summary>
        /// <param name="pattern">The string to match.</param>
        /// <returns>Skills with names that contain a given string.</returns>
        public static List<Skill> GetSkillsByPattern(string pattern)
        {
            var records = SkillWebDatabase.GetSkillRecords(pattern);
            var skills = new List<Skill>();

            foreach (var record in records)
            {
                skills.Add(Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}"));
            }

            return skills;
        }

        /// <summary>
        /// Gets all skills that match a specific size.
        /// </summary>
        /// <param name="size">The size to filter by.</param>
        /// <returns>A list of skills with the given size.</returns>
        public static List<Skill> GetSkillsBySize(Skill.Size size)
        {
            var records = SkillWebDatabase.GetSkillRecordsBySize(size);
            var skills = new List<Skill>();

            foreach (var record in records)
            {
                skills.Add(Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}"));
            }

            return skills;
        }

        /// <summary>
        /// Gets all skills that match a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <returns>A list of skills with the given tag.</returns>
        public static List<Skill> GetSkillsByTag(string tag)
        {
            var records = SkillWebDatabase.GetSkillRecordsByTag(tag);
            var skills = new List<Skill>();

            foreach (var record in records)
            {
                skills.Add(Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}"));
            }

            return skills;
        }

        /// <summary>
        /// Gets all skills that match a specific size and tag.
        /// </summary>
        /// <param name="size">The size to filter by.</param>
        /// <param name="tag">The tag to filter by (ignored if null or empty).</param>
        /// <returns>A list of skills with the given size and tag.</returns>
        public static List<Skill> GetSkillsBySizeAndTag(Skill.Size size, string tag)
        {
            var records = SkillWebDatabase.GetSkillRecordsBySizeAndTag((int)size, tag);
            var skills = new List<Skill>();

            foreach (var record in records)
            {
                skills.Add(Resources.Load<Skill>($"{Skill.resourcesPath}/{record.objectName}"));
            }

            return skills;
        }
        #endregion

        #region WEBS
        /// <summary>
        /// Gets all web graphs.
        /// </summary>
        /// <returns>A list of all web graphs.</returns>
        public static WebGraph[] GetAllWebGraphs()
        {
            var webs = Resources.LoadAll<WebGraph>(WebGraph.resourcesPath);
            return webs;
        }

        /// <summary>
        /// Gets a web graph by it's ID.
        /// </summary>
        /// <param name="id">The web graph ID.</param>
        /// <returns>A web graph with the ID or null if it does not exist.</returns>
        public static WebGraph GetWebGraph(int id)
        {
            var record = SkillWebDatabase.GetWebRecord(id);

            if (record == null)
            {
                return null;
            }

            var web = Resources.Load<WebGraph>($"{WebGraph.resourcesPath}/{record.objectName}");

            if (!web)
            {
                SkillWebLogger.LogWarning($"Skill Web: Web with the ID '{id}' doesn't exist.");
            }

            return web;
        }

        /// <summary>
        /// Gets a web graph by it's name.
        /// </summary>
        /// <param name="name">The name of the web graph.</param>
        /// <returns>A web graph with the name or null if it does not exist.</returns>
        public static WebGraph GetWebGraph(string name)
        {
            var record = SkillWebDatabase.GetWebRecord(name);

            if (record == null)
            {
                return null;
            }

            var web = Resources.Load<WebGraph>($"{WebGraph.resourcesPath}/{record.objectName}");

            if (!web)
            {
                SkillWebLogger.LogWarning($"Skill Web: Web with the name '{name}' doesn't exist.");
            }

            return web;
        }
        #endregion
    }
}