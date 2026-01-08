//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Database;
using System;

#if UNITY_EDITOR
using Esper.SkillWeb.Editor;
using System.IO;
#endif
using System.Collections.Generic;

namespace Esper.SkillWeb.Graph
{
    /// <summary>
    /// A graph that contains skills and connections.
    /// </summary>
    public class WebGraph : SkillWebObject
    {
        /// <summary>
        /// The searchable name of the web graph.
        /// </summary>
        public string webName;

        /// <summary>
        /// A guid generated when this web is changed.
        /// </summary>
        public string changeGuid;

        /// <summary>
        /// A list of all skill nodes.
        /// </summary>
        public List<SkillNode> skillNodes = new();

        /// <summary>
        /// A list of all connections.
        /// </summary>
        public List<Connection> connections = new();

        /// <summary>
        /// The dataset.
        /// </summary>
        public WebDataset dataset;

        /// <summary>
        /// The database record.
        /// </summary>
        public WebRecord DatabaseRecord
        {
            get
            {
                return new WebRecord
                {
                    id = id,
                    objectName = name,
                    webName = webName
                };
            }
        }

        /// <summary>
        /// The path to all generated objects of this type relative to the resources folder.
        /// </summary>
        public static string resourcesPath = "SkillWeb/Webs";

#if UNITY_EDITOR
        /// <summary>
        /// The directory of all generated objects of this type. Works in the editor only.
        /// </summary>
        public static string DirectoryPath { get => Path.Combine(AssetSearch.FindFolder("SkillWeb"), "Resources", "SkillWeb", "Webs"); }

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

            if (SkillWebDatabase.HasWebRecord(id))
            {
                SkillWebDatabase.DeleteWebRecord(DatabaseRecord);
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

            if (SkillWebDatabase.HasWebRecord(id))
            {
                SkillWebDatabase.UpdateWebRecord(DatabaseRecord);
            }
            else
            {
                SkillWebDatabase.InsertWebRecord(DatabaseRecord);
            }

#if UNITY_EDITOR
            if (disconnectOnComplete)
            {
                SkillWebDatabase.Disconnect();
            }
#endif
        }

        /// <summary>
        /// Gets the web graph title. This is either the display name from the dataset or the searchable web name.
        /// </summary>
        /// <returns>The title text.</returns>
        public string GetTitle()
        {
            if (dataset)
            {
                return dataset.GetDisplayName();
            }

            return webName;
        }

        /// <summary>
        /// Creates a copy of this web.
        /// </summary>
        /// <returns>The copy.</returns>
        public WebGraph CreateCopy()
        {
            List<SkillNode> skillNodes = new();
            List<Connection> connections = new();

            if (this.skillNodes != null)
            {
                foreach (var skillNode in this.skillNodes)
                {
                    var nodeCopy = skillNode.CreateCopy();
                    skillNodes.Add(nodeCopy);
                }
            }

            if (this.connections != null)
            {
                foreach (var connection in this.connections)
                {
                    var connectionCopy = connection.CreateCopy();
                    connections.Add(connectionCopy);
                }
            }

            WebGraph copy = CreateInstance<WebGraph>();
            copy.id = id;
            copy.webName = webName;
            copy.skillNodes = skillNodes;
            copy.connections = connections;
            copy.dataset = dataset;
            copy.changeGuid = SkillWebUtility.GenerateNewGuid();

            return copy;
        }

        /// <summary>
        /// Copies all data of another web, excluding ID.
        /// </summary>
        /// <param name="other">The web to copy.</param>
        public void CopyData(WebGraph other)
        { 
            webName = other.webName;
            skillNodes = other.skillNodes;
            connections = other.connections;
        }

        /// <summary>
        /// Generates the dataset with the dataset reference set from Skill Web's settings (editor only).
        /// </summary>
        public void GenerateDataset()
        {
            if (!SkillWeb.Settings.webDatasetReference)
            {
                return;
            }

#if UNITY_EDITOR
            var directoryPath = WebDataset.DirectoryPath;

            if (id >= 0)
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
#endif

            var type = SkillWeb.Settings.webDatasetReference.GetType();
            var dataset = CreateInstance(type) as WebDataset;

#if UNITY_EDITOR
            if (id >= 0)
            {
                var path = Path.Combine(directoryPath, $"{id}_WebDataset.asset");
                UnityEditor.AssetDatabase.CreateAsset(dataset, path);
                dataset = AssetSearch.Find<WebDataset>(path);
                dataset.graph = this;
                this.dataset = dataset;
                dataset.Save();
                Save();
            }
#endif
        }

        /// <summary>
        /// Creates a new instance of a Web (editor only).
        /// </summary>
        /// <returns>The created instance.</returns>
        public static WebGraph Create()
        {
#if UNITY_EDITOR
            var directoryPath = DirectoryPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
#endif

            var obj = CreateInstance<WebGraph>();
            var id = obj.GetID<WebGraph>();
            obj.id = id;
            var name = "New Web";
            obj.webName = name;
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
            webName = SanitizeName(webName);

            string name = $"{id}_{webName}";
            UnityEditor.EditorApplication.delayCall += () =>
            {
                UnityEditor.AssetDatabase.RenameAsset(GetFullPath(this), name);

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    this.name = name;
                    Save();
                };
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
        /// Gets the full path of a web (editor only).
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns>The full path to the web.</returns>
        public static string GetFullPath(WebGraph web)
        {
            return Path.Combine(DirectoryPath, $"{web.name}.asset");
        }
#endif

        protected override int GetID<T>(string pathInResources = null)
        {
            return base.GetID<T>(resourcesPath);
        }

        /// <summary>
        /// Returns an unused node ID.
        /// </summary>
        /// <returns>An unused node ID.</returns>
        public virtual int GetAvailableNodeID()
        {
            List<int> ids = new();

            foreach (var node in skillNodes)
            {
                ids.Add(node.id);
            }

            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!ids.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}