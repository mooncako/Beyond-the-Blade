#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileImportTool : EditorWindow
{
    [System.Serializable]
    public class FolderMapping
    {
        public string name;
        public string path;
        public bool isExpanded = true;
        
        public FolderMapping(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
    
    private Vector2 scrollPosition;
    private List<FolderMapping> folderMappings = new List<FolderMapping>();
    private string newMappingName = "";
    private string newMappingPath = "";
    private bool showAddMapping = false;
    
    // For file selection
    private Dictionary<int, string[]> selectedFiles = new Dictionary<int, string[]>();
    private Dictionary<int, bool> showFileList = new Dictionary<int, bool>();
    
    // Settings
    private FileImportSettings settings;
    
    [MenuItem("Tools/File Import Tool")]
    public static void ShowWindow()
    {
        GetWindow<FileImportTool>("File Import Tool");
    }
    
    private void OnEnable()
    {
        LoadSettings();
    }
    
    private void OnDisable()
    {
        SaveSettings();
    }
    
    private void LoadSettings()
    {
        settings = Resources.Load<FileImportSettings>("FileImportSettings");
        if (settings == null)
        {
            settings = CreateInstance<FileImportSettings>();
            
            // Create Resources folder if it doesn't exist
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            AssetDatabase.CreateAsset(settings, "Assets/Resources/FileImportSettings.asset");
            AssetDatabase.SaveAssets();
        }
        
        folderMappings = new List<FolderMapping>(settings.folderMappings);
    }
    
    private void SaveSettings()
    {
        if (settings != null)
        {
            settings.folderMappings = folderMappings.ToArray();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        // Header
        EditorGUILayout.LabelField("File Import Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Add new mapping section
        EditorGUILayout.BeginHorizontal();
        showAddMapping = EditorGUILayout.Foldout(showAddMapping, "Add New Folder Mapping", true);
        EditorGUILayout.EndHorizontal();
        
        if (showAddMapping)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(50));
            newMappingName = EditorGUILayout.TextField(newMappingName);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path:", GUILayout.Width(50));
            newMappingPath = EditorGUILayout.TextField(newMappingPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to relative path
                    string dataPath = Application.dataPath;
                    if (selectedPath.StartsWith(dataPath))
                    {
                        newMappingPath = "Assets" + selectedPath.Substring(dataPath.Length);
                    }
                    else
                    {
                        newMappingPath = selectedPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !string.IsNullOrEmpty(newMappingName) && !string.IsNullOrEmpty(newMappingPath);
            if (GUILayout.Button("Add Mapping"))
            {
                folderMappings.Add(new FolderMapping(newMappingName, newMappingPath));
                newMappingName = "";
                newMappingPath = "";
                SaveSettings();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        
        // Folder mappings list
        EditorGUILayout.LabelField("Folder Mappings", EditorStyles.boldLabel);
        
        if (folderMappings.Count == 0)
        {
            EditorGUILayout.HelpBox("No folder mappings configured. Add a mapping above to get started.", MessageType.Info);
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            for (int i = 0; i < folderMappings.Count; i++)
            {
                DrawFolderMapping(i);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawFolderMapping(int index)
    {
        var mapping = folderMappings[index];
        
        EditorGUILayout.BeginVertical("box");
        
        // Header with foldout
        EditorGUILayout.BeginHorizontal();
        mapping.isExpanded = EditorGUILayout.Foldout(mapping.isExpanded, mapping.name, true);
        
        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
            folderMappings.RemoveAt(index);
            SaveSettings();
            return;
        }
        EditorGUILayout.EndHorizontal();
        
        if (mapping.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // Path display
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path:", GUILayout.Width(40));
            EditorGUILayout.SelectableLabel(mapping.path, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
            
            // File selection area
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add File", GUILayout.Height(30)))
            {
                string path = EditorUtility.OpenFilePanel("Select file to import", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!selectedFiles.ContainsKey(index) || selectedFiles[index] == null)
                    {
                        selectedFiles[index] = new string[] { path };
                    }
                    else
                    {
                        var fileList = selectedFiles[index].ToList();
                        if (!fileList.Contains(path))
                        {
                            fileList.Add(path);
                            selectedFiles[index] = fileList.ToArray();
                        }
                    }
                    showFileList[index] = true;
                }
            }
            
            if (GUILayout.Button("Add Folder", GUILayout.Height(30)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select folder to import", "", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (!selectedFiles.ContainsKey(index) || selectedFiles[index] == null)
                    {
                        selectedFiles[index] = new string[] { folderPath };
                    }
                    else
                    {
                        var fileList = selectedFiles[index].ToList();
                        if (!fileList.Contains(folderPath))
                        {
                            fileList.Add(folderPath);
                            selectedFiles[index] = fileList.ToArray();
                        }
                    }
                    showFileList[index] = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Show selected files
            if (selectedFiles.ContainsKey(index) && selectedFiles[index] != null && selectedFiles[index].Length > 0)
            {
                EditorGUILayout.BeginVertical("box");
                
                showFileList[index] = EditorGUILayout.Foldout(showFileList.ContainsKey(index) ? showFileList[index] : true, 
                    $"Selected Files ({selectedFiles[index].Length})");
                
                if (showFileList[index])
                {
                    EditorGUI.indentLevel++;
                    
                    for (int fileIndex = 0; fileIndex < selectedFiles[index].Length; fileIndex++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(Path.GetFileName(selectedFiles[index][fileIndex]), EditorStyles.miniLabel);
                        if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        {
                            var fileList = selectedFiles[index].ToList();
                            fileList.RemoveAt(fileIndex);
                            selectedFiles[index] = fileList.ToArray();
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Import Selected Files", GUILayout.Height(25)))
                {
                    ImportFiles(selectedFiles[index], mapping.path);
                    selectedFiles[index] = new string[0]; // Clear selection after import
                }
                GUI.backgroundColor = Color.white;
                
                if (GUILayout.Button("Clear Selection", GUILayout.Width(100), GUILayout.Height(25)))
                {
                    selectedFiles[index] = new string[0];
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
            
            // Show recent imports or additional info
            if (AssetDatabase.IsValidFolder(mapping.path))
            {
                EditorGUILayout.LabelField("✓ Folder exists", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField("⚠ Folder does not exist", EditorStyles.miniLabel);
                if (GUILayout.Button("Create Folder"))
                {
                    CreateFolderPath(mapping.path);
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    

    
    private void ImportFiles(string[] sourcePaths, string targetPath)
    {
        if (!AssetDatabase.IsValidFolder(targetPath))
        {
            if (EditorUtility.DisplayDialog("Create Folder", 
                $"Target folder '{targetPath}' does not exist. Create it?", "Create", "Cancel"))
            {
                CreateFolderPath(targetPath);
            }
            else
            {
                return;
            }
        }
        
        int importedCount = 0;
        List<string> errors = new List<string>();
        
        foreach (string sourcePath in sourcePaths)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    string fileName = Path.GetFileName(sourcePath);
                    string destinationPath = Path.Combine(targetPath, fileName).Replace("\\", "/");
                    
                    // Check if file already exists
                    if (File.Exists(destinationPath))
                    {
                        if (!EditorUtility.DisplayDialog("File Exists", 
                            $"File '{fileName}' already exists in '{targetPath}'. Overwrite?", "Overwrite", "Skip"))
                        {
                            continue;
                        }
                    }
                    
                    File.Copy(sourcePath, destinationPath, true);
                    importedCount++;
                }
                else if (Directory.Exists(sourcePath))
                {
                    string folderName = Path.GetFileName(sourcePath);
                    string destinationPath = Path.Combine(targetPath, folderName).Replace("\\", "/");
                    
                    CopyDirectory(sourcePath, destinationPath);
                    importedCount++;
                }
            }
            catch (System.Exception ex)
            {
                errors.Add($"Failed to import '{Path.GetFileName(sourcePath)}': {ex.Message}");
            }
        }
        
        // Refresh the asset database
        AssetDatabase.Refresh();
        
        // Show results
        if (errors.Count > 0)
        {
            string errorMessage = "Import completed with errors:\n" + string.Join("\n", errors);
            EditorUtility.DisplayDialog("Import Results", errorMessage, "OK");
        }
        else if (importedCount > 0)
        {
            EditorUtility.DisplayDialog("Import Successful", 
                $"Successfully imported {importedCount} item(s) to '{targetPath}'", "OK");
        }
    }
    
    private void CreateFolderPath(string path)
    {
        string[] folders = path.Split('/');
        string currentPath = "";
        
        for (int i = 0; i < folders.Length; i++)
        {
            if (i == 0)
            {
                currentPath = folders[i];
            }
            else
            {
                string newFolder = folders[i];
                if (!AssetDatabase.IsValidFolder(currentPath + "/" + newFolder))
                {
                    AssetDatabase.CreateFolder(currentPath, newFolder);
                }
                currentPath += "/" + newFolder;
            }
        }
        
        AssetDatabase.Refresh();
    }
    
    private void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
        
        // Copy files
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDir, fileName);
            File.Copy(file, destFile, true);
        }
        
        // Copy subdirectories
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(subDir);
            string destSubDir = Path.Combine(destDir, dirName);
            CopyDirectory(subDir, destSubDir);
        }
    }
}

[CreateAssetMenu(fileName = "FileImportSettings", menuName = "Tools/File Import Settings")]
public class FileImportSettings : ScriptableObject
{
    public FileImportTool.FolderMapping[] folderMappings = new FileImportTool.FolderMapping[0];
}
#endif 