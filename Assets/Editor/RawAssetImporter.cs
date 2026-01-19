using System.IO;
using UnityEditor;
using UnityEngine;

public class RawAssetImporter : EditorWindow
{
    [MenuItem("Tools/Raw Asset Importer")]
    public static void ShowWindow()
    {
        GetWindow<RawAssetImporter>("Raw Asset Importer");
    }

    private string _targetFolder = "Assets/Art/Environment/Props";

    private void OnGUI()
    {
        GUILayout.Label("Raw Asset Importer", EditorStyles.boldLabel);
        
        using(new EditorGUI.DisabledScope(true))
            _targetFolder = EditorGUILayout.TextField("Target Folder", _targetFolder);
        
        if(GUILayout.Button("Select Target Folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
            if (!string.IsNullOrEmpty(folder))
            {
                _targetFolder = folder.StartsWith(Application.dataPath) 
                    ? "Assets" + folder[Application.dataPath.Length..] 
                    : _targetFolder;
            }
        }

        if (GUILayout.Button("Select and Import FBX"))
        {
            // A disk folder to start browsing from (outside Assets is fine)
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string rawFolderPath = Path.Combine(projectRoot, "raw");

            if (!Directory.Exists(rawFolderPath))
                Directory.CreateDirectory(rawFolderPath);

            string pickedPath = EditorUtility.OpenFilePanel("Select FBX Asset", rawFolderPath, "fbx");
            if (string.IsNullOrEmpty(pickedPath))
                return;

            // 1) Ensure target folder is valid and in Assets
            if (string.IsNullOrEmpty(_targetFolder) || !_targetFolder.StartsWith("Assets/"))
            {
                Debug.LogError($"_targetFolder must be an 'Assets/...' folder. Current: {_targetFolder}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(_targetFolder))
            {
                Debug.LogError($"Target folder does not exist in project: {_targetFolder}");
                return;
            }

            // 2) Convert picked path to an AssetDatabase path:
            //    - If external, copy into Assets (or your raw folder under project root if it's inside Assets).
            string assetPath = ToAssetPathOrCopyIntoAssets(pickedPath);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Could not resolve FBX into an AssetDatabase path (Assets/...).");
                return;
            }

            // 3) Make sure Unity imports it
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            // 4) Load FBX as a GameObject (FBX model asset)
            GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (fbxAsset == null)
            {
                Debug.LogError($"Failed to load FBX as GameObject at: {assetPath}");
                return;
            }

            // 5) Instantiate and save as prefab
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset);
            try
            {
                string prefabAssetPath = $"{_targetFolder}/{fbxAsset.name}.prefab".Replace("\\", "/");
                prefabAssetPath = AssetDatabase.GenerateUniqueAssetPath(prefabAssetPath);

                PrefabUtility.SaveAsPrefabAsset(instance, prefabAssetPath);
                Debug.Log($"FBX imported as prefab to: {prefabAssetPath}");
            }
            finally
            {
                if (instance != null)
                    Object.DestroyImmediate(instance);
            }

            AssetDatabase.Refresh();
        }
    }

    private static string ToAssetPathOrCopyIntoAssets(string pickedAbsolutePath)
    {
        pickedAbsolutePath = Path.GetFullPath(pickedAbsolutePath);

        string assetsAbs = Path.GetFullPath(Application.dataPath);
        string projectRootAbs = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        // Case A: Picked file already under Assets -> convert absolute -> "Assets/..."
        if (pickedAbsolutePath.StartsWith(assetsAbs))
        {
            string rel = pickedAbsolutePath.Substring(assetsAbs.Length).Replace("\\", "/");
            return ("Assets" + rel).Replace("\\", "/");
        }

        // Case B: External file -> copy it into Assets/RawImports (or any folder you prefer)
        const string importFolder = "Assets/RawImports";
        if (!AssetDatabase.IsValidFolder(importFolder))
        {
            // Create Assets/RawImports
            AssetDatabase.CreateFolder("Assets", "RawImports");
        }

        string fileName = Path.GetFileName(pickedAbsolutePath);
        string destAssetPath = $"{importFolder}/{fileName}".Replace("\\", "/");
        destAssetPath = AssetDatabase.GenerateUniqueAssetPath(destAssetPath);

        string destAbs = Path.GetFullPath(Path.Combine(projectRootAbs, destAssetPath));
        File.Copy(pickedAbsolutePath, destAbs, overwrite: false);

        // Return the Assets/ path so AssetDatabase can import it
        return destAssetPath;
    }
}
