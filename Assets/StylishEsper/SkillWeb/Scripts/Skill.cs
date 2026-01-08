//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Database;
#if UNITY_EDITOR
using Esper.SkillWeb.Editor;
using System.IO;
#endif
using UnityEngine;
using UnityEngine.Video;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Skill data container.
    /// </summary>
    public class Skill : SkillWebObject
    {
        /// <summary>
        /// The searchable name of the skill.
        /// </summary>
        public string skillName;

        /// <summary>
        /// The max skill level.
        /// </summary>
        public int maxLevel = 1;

        /// <summary>
        /// The level the player must reach to be able to unlock this skill.
        /// </summary>
        public int levelRequirement;

        /// <summary>
        /// The tag index.
        /// </summary>
        public int tagIndex;

        /// <summary>
        /// The icon to display when the skill is locked.
        /// </summary>
        public SkillIcon lockedIcon;

        /// <summary>
        /// The icon to disply when the skill is unlocked.
        /// </summary>
        public SkillIcon unlockedIcon;

        /// <summary>
        /// The icon to display when the skill is obtained.
        /// </summary>
        public SkillIcon obtainedIcon;

        /// <summary>
        /// The icon to display when the skill is maxed.
        /// </summary>
        public SkillIcon maxedIcon;

        /// <summary>
        /// The size of the skill in the UI.
        /// </summary>
        public Size size;

        /// <summary>
        /// A video clip that demonstrates the skill.
        /// </summary>
        public VideoClip demoClip;

        /// <summary>
        /// The dataset.
        /// </summary>
        public SkillDataset dataset;

        /// <summary>
        /// The skill tag.
        /// </summary>
        public string Tag 
        {
            get
            {
                try
                {
                    return SkillWeb.Settings.tags[tagIndex - 1];
                }
                catch
                {
                    return "None";
                }
            }
            set
            {
                try
                {
                    tagIndex = SkillWeb.Settings.tags.IndexOf(value);
                }
                catch
                {
                    SkillWebLogger.LogWarning($"Skill: The tag {value} doesn't exist.");
                }
            }
        }

        /// <summary>
        /// The database record.
        /// </summary>
        public SkillRecord DatabaseRecord
        {
            get
            {
                return new SkillRecord
                {
                    id = id,
                    objectName = name,
                    skillName = skillName,
                    size = (int)size,
                    tag = Tag
                };
            }
        }

        /// <summary>
        /// The path to all generated objects of this type relative to the resources folder.
        /// </summary>
        public static string resourcesPath = "SkillWeb/Skills";

#if UNITY_EDITOR
        /// <summary>
        /// The directory of all generated objects of this type. Works in the editor only.
        /// </summary>
        public static string DirectoryPath { get => Path.Combine(AssetSearch.FindFolder("SkillWeb"), "Resources", "SkillWeb", "Skills"); }

        public void SetClip(VideoClip videoClip)
        {
            
        }

        /// <summary>
        /// Deletes the record of this object from the database.
        /// </summary>
        public void DeleteDatabaseRecord()
        {
            bool disconnectOnComplete = false;

            if (!SkillWebDatabase.IsConnected)
            {
                SkillWebDatabase.Initialize();
                disconnectOnComplete = true;
            }

            if (SkillWebDatabase.HasSkillRecord(id))
            {
                SkillWebDatabase.DeleteSkillRecord(DatabaseRecord);
            }

            if (disconnectOnComplete)
            {
                SkillWebDatabase.Disconnect();
            }
        }
#endif

        /// <summary>
        /// Updates the record of this object in the database.
        /// </summary>
        public void UpdateDatabaseRecord()
        {
#if UNITY_EDITOR
            bool disconnectOnComplete = false;

            if (!SkillWebDatabase.IsConnected)
            {
                SkillWebDatabase.Initialize();
                disconnectOnComplete = true;
            }
#endif

            if (SkillWebDatabase.HasSkillRecord(id))
            {
                SkillWebDatabase.UpdateSkillRecord(DatabaseRecord);
            }
            else
            {
                SkillWebDatabase.InsertSkillRecord(DatabaseRecord);
            }

#if UNITY_EDITOR
            if (disconnectOnComplete)
            {
                SkillWebDatabase.Disconnect();
            }
#endif
        }

        /// <summary>
        /// Gets an icon for a specific state.
        /// </summary>
        /// <param name="state">The skill state.</param>
        /// <param name="useFallback">If the requested skill icon doesn't have a sprite set, a fallback should be returned.</param>
        /// <returns>Returns the skill icon for the specific state.</returns>
        public SkillIcon GetIcon(State state, bool useFallback)
        {
            SkillIcon skillIcon = default;

            switch (state)
            {
                case State.Locked:
                    skillIcon = lockedIcon;
                    break;

                case State.Unlocked:
                    skillIcon = unlockedIcon;
                    break;

                case State.Obtained:
                    skillIcon = obtainedIcon;
                    break;

                case State.Maxed:
                    skillIcon = maxedIcon;
                    break;
            }

            if (useFallback && !skillIcon.icon)
            {
                if (!skillIcon.icon)
                {
                    if (state == State.Maxed)
                    {
                        if (obtainedIcon.icon)
                        {
                            skillIcon = obtainedIcon;
                        }
                        else if (unlockedIcon.icon)
                        {
                            skillIcon = unlockedIcon;
                        }
                        else if (lockedIcon.icon)
                        {
                            skillIcon = lockedIcon;
                        }
                    }
                    else if (state == State.Obtained)
                    {
                        if (unlockedIcon.icon)
                        {
                            skillIcon = unlockedIcon;
                        }
                        else if (lockedIcon.icon)
                        {
                            skillIcon = lockedIcon;
                        }
                        else if (maxedIcon.icon)
                        {
                            skillIcon = maxedIcon;
                        }
                    }
                    else if (state == State.Unlocked)
                    {
                        if (lockedIcon.icon)
                        {
                            skillIcon = lockedIcon;
                        }
                        else if (obtainedIcon.icon)
                        {
                            skillIcon = obtainedIcon;
                        }
                        else if (maxedIcon.icon)
                        {
                            skillIcon = maxedIcon;
                        }
                    }
                    else if (state == State.Locked)
                    {
                        if (unlockedIcon.icon)
                        {
                            skillIcon = unlockedIcon;
                        }
                        else if (obtainedIcon.icon)
                        {
                            skillIcon = obtainedIcon;
                        }
                        else if (maxedIcon.icon)
                        {
                            skillIcon = maxedIcon;
                        }
                    }
                }
            }

            return skillIcon;
        }

        /// <summary>
        /// Generates the dataset with the dataset reference set from Skill Web's settings (editor only).
        /// </summary>
        public void GenerateDataset()
        {
            if (!SkillWeb.Settings.skillDatasetReference)
            {
                return;
            }

#if UNITY_EDITOR
            var directoryPath = SkillDataset.DirectoryPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
#endif

            var type = SkillWeb.Settings.skillDatasetReference.GetType();
            var dataset = CreateInstance(type) as SkillDataset;

#if UNITY_EDITOR
            var path = Path.Combine(directoryPath, $"{id}_SkillDataset.asset");
            UnityEditor.AssetDatabase.CreateAsset(dataset, path);
            dataset = AssetSearch.Find<SkillDataset>(path);
            dataset.skill = this;
            this.dataset = dataset;
            dataset.Save();
            Save();
#endif
        }

        /// <summary>
        /// Creates a new instance of a skill (editor only).
        /// </summary>
        /// <returns>The created instance.</returns>
        public static Skill Create()
        {
#if UNITY_EDITOR
            var directoryPath = DirectoryPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
#endif

            var obj = CreateInstance<Skill>();
            var id = obj.GetID<Skill>();
            obj.id = id;
            var name = "New Skill";
            obj.skillName = name;
            obj.lockedIcon.color = Color.white;
            obj.unlockedIcon.color = Color.white;
            obj.obtainedIcon.color = Color.white;
            obj.maxedIcon.color = Color.white;
            obj.GenerateDataset();

#if UNITY_EDITOR
            var path = Path.Combine(directoryPath, $"{id}_{name}.asset");
            UnityEditor.AssetDatabase.CreateAsset(obj, path);
            obj.Save();
#endif

            return obj;
        }

        /// <summary>
        /// Updates the name of the asset (editor only).
        /// </summary>
        public void UpdateAssetName()
        {
#if UNITY_EDITOR
            skillName = SanitizeName(skillName);

            string name = $"{id}_{skillName}";
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    UnityEditor.AssetDatabase.RenameAsset(GetFullPath(this), name);

                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        if (this != null)
                        {
                            this.name = name;
                            Save();
                        }
                    };
                }
            };
#endif
        }

        public override void Save()
        {
#if UNITY_EDITOR
            base.Save();
            UpdateDatabaseRecord();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gets the full path of a skill (editor only).
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <returns>The full path to the skill.</returns>
        public static string GetFullPath(Skill skill)
        {
            return Path.Combine(DirectoryPath, $"{skill.name}.asset");
        }
#endif

        protected override int GetID<T>(string pathInResources = null)
        {
            return base.GetID<T>(resourcesPath);
        }

        /// <summary>
        /// Represents a skill icon.
        /// </summary>
        [System.Serializable]
        public struct SkillIcon
        {
            /// <summary>
            /// The icon.
            /// </summary>
            public Sprite icon;

            /// <summary>
            /// The color tint of the icon.
            /// </summary>
            public Color color;
        }

        /// <summary>
        /// The state of a skill.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Currently unobtainable. A skill is unobtainable if the level or connection requirement is not met.
            /// </summary>
            Locked,

            /// <summary>
            /// The skill can be obtained.
            /// </summary>
            Unlocked,

            /// <summary>
            /// The skill has been obtained.
            /// </summary>
            Obtained,

            /// <summary>
            /// The skill's level has been maxed.
            /// </summary>
            Maxed
        }

        /// <summary>
        /// The size of a skill.
        /// </summary>
        public enum Size
        {
            Tiny,
            Small,
            Medium,
            Large,
            Giant
        }
    }
}