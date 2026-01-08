//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Esper.SkillWeb.Database
{
    /// <summary>
    /// Skill Web's database handler. Executes all database queries.
    /// </summary>
    public static class SkillWebDatabase
    {
        /// <summary>
        /// The database directory path in the editor.
        /// </summary>
        public static string DatabaseEditorDirectoryPath { get; private set; } = Application.streamingAssetsPath;

        /// <summary>
        /// The database directory path at runtime.
        /// </summary>
        public static string DatabaseRuntimeDirectoryPath { get; private set; } = Application.persistentDataPath;

        public static string databaseName;
        private readonly static string skillTableName = "skills";
        private readonly static string webTableName = "webs";

        private static SQLiteConnection connection;

        /// <summary>
        /// If there is an active connection to the database.
        /// </summary>
        public static bool IsConnected { get => connection != null; }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        public static void Initialize()
        {
            if (IsConnected)
            {
                return;
            }

            string databaseFullPath = string.Empty;

            bool updateDatabase = false;
            databaseName = $"{SkillWeb.Settings.databaseName}.db";


#if UNITY_EDITOR
            databaseFullPath = Path.Combine(DatabaseEditorDirectoryPath, databaseName);

            // Create database directory if it does not exists
            if (!Directory.Exists(DatabaseEditorDirectoryPath))
            {
                Directory.CreateDirectory(DatabaseEditorDirectoryPath);
            }

            // Create database file if it does not exist
            if (!File.Exists(databaseFullPath))
            {
                File.WriteAllBytes(databaseFullPath, new byte[] { });
                updateDatabase = true;
            }
#else
            databaseFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, databaseName);

            // Create database directory if it does not exist
            if (!Directory.Exists(DatabaseRuntimeDirectoryPath))
            {
                Directory.CreateDirectory(DatabaseRuntimeDirectoryPath);
            }

            if (!File.Exists(databaseFullPath))
            {
                CreateRuntimeDatabase();
            }

            // Always update the database in case any changes were made
            updateDatabase = true;
#endif

            connection = new SQLiteConnection(databaseFullPath);

            // Create tables if they don't exist
            CreateSkillTable();
            CreateWebTable();

            if (updateDatabase)
            {
                // Clear tables to ensure deleted objects are removed
                ClearTable(skillTableName);
                ClearTable(webTableName);

                var skills = SkillWeb.GetAllSkills();

                foreach (var item in skills)
                {
                    item.UpdateDatabaseRecord();
                }

                var webs = SkillWeb.GetAllWebGraphs();

                foreach (var item in webs)
                {
                    item.UpdateDatabaseRecord();
                }

                skills = null;
                webs = null;

                Resources.UnloadUnusedAssets();
            }

            if (Application.isPlaying)
            {
                SkillWebLogger.Log("Skill Web: Initialized database connection.");
            }
        }

        /// <summary>
        /// Disconnects from the database.
        /// </summary>
        public static void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            connection.Close();
            connection.Dispose();
            connection = null;

            if (Application.isPlaying)
            {
                DeleteRuntimeDatabase();
                SkillWebLogger.Log("Skill Web: Disconnected from database.");
            }
        }

        /// <summary>
        /// Copies the database from StreamingAssets and stores it in the user's system. This is
        /// necessary to have a connection with the database at runtime.
        /// </summary>
        public static void CreateRuntimeDatabase()
        {
            string databaseEditorFullPath = Path.Combine(DatabaseEditorDirectoryPath, databaseName);
            string databaseRuntimeFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, databaseName);
            string uri = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            uri = "file://" + databaseEditorFullPath;
#else
            uri = databaseEditorFullPath;
#endif

            // Copy file from default database (from StreamingAssets)
            var loadingRequest = UnityWebRequest.Get(uri);
            loadingRequest.SendWebRequest();

            while (!loadingRequest.isDone)
            {
                switch (loadingRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        SkillWebLogger.LogError("Skill Web: Failed to create runtime database (connection error).");
                        return;

                    case UnityWebRequest.Result.ProtocolError:
                        SkillWebLogger.LogError("Skill Web: Failed to create runtime database (protocol error).");
                        return;

                    case UnityWebRequest.Result.DataProcessingError:
                        SkillWebLogger.LogError("Skill Web: Failed to create runtime database (data processing error).");
                        return;
                }
            }

            if (loadingRequest.downloadHandler.data == null || loadingRequest.downloadHandler.data.Length == 0)
            {
                SkillWebLogger.LogError("Skill Web: Failed to create runtime database. This usually means that the editor " +
                    "database has not been created or found. Try navigating to Window > Skill Web > Settings, click " +
                    "'Validate', and then try again. If this persists, consider contacting the developer.");
                return;
            }

            File.WriteAllBytes(databaseRuntimeFullPath, loadingRequest.downloadHandler.data);
            loadingRequest.Dispose();
        }


        /// <summary>
        /// Deletes the runtime database. This requires a disconnection first.
        /// </summary>
        public static void DeleteRuntimeDatabase()
        {
            var dbName = string.IsNullOrEmpty(databaseName) ? SkillWeb.Settings.databaseName : databaseName;
            string databaseFullPath = Path.Combine(DatabaseRuntimeDirectoryPath, dbName);
            File.Delete(databaseFullPath);
        }

        /// <summary>
        /// Deletes the editor database. This requires a disconnection first.
        /// </summary>
        public static void DeleteEditorDatabase()
        {
            var dbName = string.IsNullOrEmpty(databaseName) ? SkillWeb.Settings.databaseName : databaseName;
            string databaseFullPath = Path.Combine(DatabaseEditorDirectoryPath, dbName);
            File.Delete(databaseFullPath);
            File.Delete($"{databaseFullPath}.meta");
        }

        /// <summary>
        /// Deletes all records from a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public static void ClearTable(string tableName)
        {
            connection.Execute($"DELETE FROM {tableName}");
        }

        #region "SKILLS"
        /// <summary>
        /// Creates the skill table if it doesn't exist.
        /// </summary>
        public static void CreateSkillTable()
        {
            connection.Execute($"CREATE TABLE IF NOT EXISTS {skillTableName} (id INTEGER NOT NULL PRIMARY KEY, object_name VARCHAR(255), skill_name VARCHAR(255), size INTEGER, tag VARCHAR(255))");
        }

        /// <summary>
        /// Adds the "size" and "tag" column if it does not already exist.
        /// </summary>
        public static void EnsureColumnsExists()
        {
            try
            {
                connection.Execute($"ALTER TABLE {skillTableName} ADD COLUMN size INTEGER");
            }
            catch (SQLiteException ex)
            {
                // SQLite error code 1 with "duplicate column name" means it already exists.
                if (!ex.Message.Contains("duplicate column name", StringComparison.OrdinalIgnoreCase))
                {
                    throw; // rethrow if it's another issue
                }
            }

            try
            {
                connection.Execute($"ALTER TABLE {skillTableName} ADD COLUMN tag VARCHAR(255)");
            }
            catch (SQLiteException ex)
            {
                // SQLite error code 1 with "duplicate column name" means it already exists.
                if (!ex.Message.Contains("duplicate column name", StringComparison.OrdinalIgnoreCase))
                {
                    throw; // rethrow if it's another issue
                }
            }
        }

        /// <summary>
        /// Inserts a skill record.
        /// </summary>
        /// <param name="skillRecord">The skill record.</param>
        public static void InsertSkillRecord(SkillRecord skillRecord)
        {
#if UNITY_EDITOR
            EnsureColumnsExists();
#endif

            var cmd = connection.CreateCommand($"INSERT INTO {skillTableName} VALUES (@id, @object_name, @skill_name, @size, @tag)");
            cmd.Bind("@id", skillRecord.id);
            cmd.Bind("@object_name", skillRecord.objectName);
            cmd.Bind("@skill_name", skillRecord.skillName);
            cmd.Bind("@size", skillRecord.size);
            cmd.Bind("@tag", skillRecord.tag);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates an existing skill record.
        /// </summary>
        /// <param name="skillRecord">The skill record.</param>
        public static void UpdateSkillRecord(SkillRecord skillRecord)
        {
#if UNITY_EDITOR
            EnsureColumnsExists();
#endif

            var cmd = connection.CreateCommand($"UPDATE {skillTableName} SET object_name = @object_name, skill_name = @skill_name, size = @size, tag = @tag WHERE id = @id");
            cmd.Bind("@id", skillRecord.id);
            cmd.Bind("@object_name", skillRecord.objectName);
            cmd.Bind("@skill_name", skillRecord.skillName);
            cmd.Bind("@size", skillRecord.size);
            cmd.Bind("@tag", skillRecord.tag);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing skill record.
        /// </summary>
        /// <param name="skillRecord">The skill record.</param>
        public static void DeleteSkillRecord(SkillRecord skillRecord)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {skillTableName} WHERE id = @id");
            cmd.Bind("@id", skillRecord.id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing skill record.
        /// </summary>
        /// <param name="id">The skill record ID.</param>
        public static void DeleteSkillRecord(int id)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {skillTableName} WHERE id = @id");
            cmd.Bind("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a skill record exists.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>True if the record exists. Otherwise, false.</returns>
        public static bool HasSkillRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<SkillRecord>();

            return records.Count > 0;
        }

        /// <summary>
        /// Gets a skill record.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>A skill record with the ID or null if it doesn't exist.</returns>
        public static SkillRecord GetSkillRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<SkillRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets a skill record.
        /// </summary>
        /// <param name="skillName">The skill name.</param>
        /// <returns>A skill record with the ID or null if it doesn't exist.</returns>
        public static SkillRecord GetSkillRecord(string skillName)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE skill_name = @skill_name");
            cmd.Bind("@skill_name", skillName);

            var records = cmd.ExecuteQuery<SkillRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets all skill records with names that contain a given string.
        /// </summary>
        /// <param name="pattern">The string to match.</param>
        /// <returns>Skill records with names that contain a given string.</returns>
        public static List<SkillRecord> GetSkillRecords(string pattern)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE LOWER(skill_name) LIKE LOWER(@pattern)");
            cmd.Bind("@pattern", $"%{pattern}%");

            var records = cmd.ExecuteQuery<SkillRecord>();
            return records;
        }

        /// <summary>
        /// Gets all skill records that match a specific size.
        /// </summary>
        /// <param name="size">The size to filter by.</param>
        /// <returns>A list of skill records with the given size.</returns>
        public static List<SkillRecord> GetSkillRecordsBySize(Skill.Size size)
        {
#if UNITY_EDITOR
            EnsureColumnsExists();
#endif

            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE size = @size");
            cmd.Bind("@size", (int)size);

            return cmd.ExecuteQuery<SkillRecord>();
        }

        /// <summary>
        /// Gets all skill records that match a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <returns>A list of skill records with the given tag.</returns>
        public static List<SkillRecord> GetSkillRecordsByTag(string tag)
        {
#if UNITY_EDITOR
            EnsureColumnsExists();
#endif

            var cmd = connection.CreateCommand($"SELECT * FROM {skillTableName} WHERE tag = @tag");
            cmd.Bind("@tag", tag);

            return cmd.ExecuteQuery<SkillRecord>();
        }

        /// <summary>
        /// Gets all skill records that match a specific size and tag.
        /// </summary>
        /// <param name="size">The size to filter by (ignored if -1).</param>
        /// <param name="tag">The tag to filter by (ignored if null or empty).</param>
        /// <returns>A list of skill records with the given size and tag.</returns>
        public static List<SkillRecord> GetSkillRecordsBySizeAndTag(int size, string tag)
        {
#if UNITY_EDITOR
            EnsureColumnsExists();
#endif

            // Base query
            var sql = $"SELECT * FROM {skillTableName}";
            var whereClauses = new List<string>();
            var cmd = connection.CreateCommand(sql);

            // Add filters dynamically
            if (size != -1)
            {
                whereClauses.Add("size = @size");
                cmd.Bind("@size", size);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                whereClauses.Add("tag = @tag");
                cmd.Bind("@tag", tag);
            }

            // Append WHERE if needed
            if (whereClauses.Count > 0)
            {
                cmd.CommandText += " WHERE " + string.Join(" AND ", whereClauses);
            }

            return cmd.ExecuteQuery<SkillRecord>();
        }
        #endregion

        #region "WEBS"
        /// <summary>
        /// Creates the web table if it doesn't exist.
        /// </summary>
        public static void CreateWebTable()
        {
            connection.Execute($"CREATE TABLE IF NOT EXISTS {webTableName} (id INTEGER NOT NULL PRIMARY KEY, object_name VARCHAR(255), web_name VARCHAR(255))");
        }

        /// <summary>
        /// Inserts a web record.
        /// </summary>
        /// <param name="webRecord">The web record.</param>
        public static void InsertWebRecord(WebRecord webRecord)
        {
            var cmd = connection.CreateCommand($"INSERT INTO {webTableName} VALUES (@id, @object_name, @web_name)");
            cmd.Bind("@id", webRecord.id);
            cmd.Bind("@object_name", webRecord.objectName);
            cmd.Bind("@web_name", webRecord.webName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates an existing web record.
        /// </summary>
        /// <param name="webRecord">The web record.</param>
        public static void UpdateWebRecord(WebRecord webRecord)
        {
            var cmd = connection.CreateCommand($"UPDATE {webTableName} SET object_name = @object_name, web_name = @web_name WHERE id = @id");
            cmd.Bind("@id", webRecord.id);
            cmd.Bind("@object_name", webRecord.objectName);
            cmd.Bind("@web_name", webRecord.webName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing web record.
        /// </summary>
        /// <param name="webRecord">The web record.</param>
        public static void DeleteWebRecord(WebRecord webRecord)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {webTableName} WHERE id = @id");
            cmd.Bind("@id", webRecord.id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an existing web record.
        /// </summary>
        /// <param name="id">The web record ID.</param>
        public static void DeleteWebRecord(int id)
        {
            var cmd = connection.CreateCommand($"DELETE FROM {webTableName} WHERE id = @id");
            cmd.Bind("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a web record exists.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>True if the record exists. Otherwise, false.</returns>
        public static bool HasWebRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {webTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<WebRecord>();

            return records.Count > 0;
        }

        /// <summary>
        /// Gets a web record.
        /// </summary>
        /// <param name="id">The web ID.</param>
        /// <returns>A web record with the ID or null if it doesn't exist.</returns>
        public static WebRecord GetWebRecord(int id)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {webTableName} WHERE id = @id");
            cmd.Bind("@id", id);

            var records = cmd.ExecuteQuery<WebRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }

        /// <summary>
        /// Gets a web record.
        /// </summary>
        /// <param name="webName">The web web name.</param>
        /// <returns>A web record with the ID or null if it doesn't exist.</returns>
        public static WebRecord GetWebRecord(string webName)
        {
            var cmd = connection.CreateCommand($"SELECT * FROM {webTableName} WHERE web_name = @web_name");
            cmd.Bind("@web_name", webName);

            var records = cmd.ExecuteQuery<WebRecord>();

            if (records.Count > 0)
            {
                return records.First();
            }

            return null;
        }
        #endregion
    }
}