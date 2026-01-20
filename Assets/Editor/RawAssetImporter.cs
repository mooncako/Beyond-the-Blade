using System.IO;
using UnityEditor;
using UnityEngine;

public class RawAssetImporter : EditorWindow
{
    [MenuItem("Tools/Raw Asset Importer", false, 20)]
    public static void ShowWindow()
    {
        GetWindow<RawAssetImporter>("Raw Asset Importer");
    }

    private static string _targetAssetFolder = "Assets/Art/Environment/Props/";
    private static string _targetPrefabFolder = "Assets/Prefabs/Environment/";

    private void OnGUI()
    {
        GUILayout.Label("Raw Asset Importer", EditorStyles.boldLabel);
        
        using(new EditorGUI.DisabledScope(true))
        {
            _targetAssetFolder = EditorGUILayout.TextField("Target Asset Folder", _targetAssetFolder);
            _targetPrefabFolder = EditorGUILayout.TextField("Target Prefab Folder", _targetPrefabFolder);
        }
            
        if(GUILayout.Button("Select Target Asset Folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Target Asset Folder", "Assets/Art/Environment/Props/", "");
            if (!string.IsNullOrEmpty(folder))
            {
                _targetAssetFolder = folder.StartsWith(Application.dataPath) 
                    ? "Assets" + folder[Application.dataPath.Length..] 
                    : _targetAssetFolder;
            }
        }

        if(GUILayout.Button("Select Target Prefab Folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Target Prefab Folder", "Assets/Prefabs/Environment/", "");
            if (!string.IsNullOrEmpty(folder))
            {
                _targetPrefabFolder = folder.StartsWith(Application.dataPath) 
                    ? "Assets" + folder[Application.dataPath.Length..] 
                    : _targetPrefabFolder;
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
            if (string.IsNullOrEmpty(_targetPrefabFolder) || !_targetPrefabFolder.StartsWith("Assets/"))
            {
                Debug.LogError($"_targetFolder must be an 'Assets/...' folder. Current: {_targetPrefabFolder}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(_targetPrefabFolder))
            {
                Debug.LogError($"Target folder does not exist in project: {_targetPrefabFolder}");
                return;
            }

            if (string.IsNullOrEmpty(_targetAssetFolder) || !_targetAssetFolder.StartsWith("Assets/"))
            {
                Debug.LogError($"_targetFolder must be an 'Assets/...' folder. Current: {_targetAssetFolder}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(_targetAssetFolder))
            {
                Debug.LogError($"Target folder does not exist in project: {_targetAssetFolder}");
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

            // 5) Create parent object, add FBX as child, and save as prefab
            GameObject parent = new GameObject(fbxAsset.name);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset, parent.transform);
            
            // Set layer to "Environment" and add colliders to all objects
            int environmentLayer = LayerMask.NameToLayer("Environment");
            if (environmentLayer == -1)
            {
                Debug.LogWarning("Layer 'Environment' does not exist. Please create it in Tags & Layers.");
            }
            else
            {
                foreach (Transform child in instance.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = environmentLayer;
                    
                    // Add MeshCollider if the object has a MeshFilter and doesn't already have a collider
                    MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                    if (meshFilter != null && child.GetComponent<Collider>() == null)
                    {
                        MeshCollider collider = child.gameObject.AddComponent<MeshCollider>();
                        collider.convex = false;
                    }
                }
            }
            GameObject prefab;
            try
            {
                string prefabAssetPath = $"{_targetPrefabFolder}/{fbxAsset.name}.prefab".Replace("\\", "/");

                if(File.Exists(prefabAssetPath))
                {
                    Debug.LogWarning($"Prefab already exists at: {prefabAssetPath}");
                    if (parent != null)
                        DestroyImmediate(parent);
                    return;
                }
                prefabAssetPath = AssetDatabase.GenerateUniqueAssetPath(prefabAssetPath);

                prefab = PrefabUtility.SaveAsPrefabAsset(parent, prefabAssetPath);
                Debug.Log($"FBX imported as prefab to: {prefabAssetPath}");
            }
            finally
            {
                if (parent != null)
                    DestroyImmediate(parent);
            }

            AssetDatabase.Refresh();
            if(prefab != null)
            {
                Selection.activeObject = prefab;
                EditorGUIUtility.PingObject(prefab);
            }
        }
    }

    private static string ToAssetPathOrCopyIntoAssets(string pickedAbsolutePath)
    {
        pickedAbsolutePath = Path.GetFullPath(pickedAbsolutePath);
        string projectRootAbs = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));


        string fileName = Path.GetFileName(pickedAbsolutePath);
        string destAssetPath = $"{_targetAssetFolder}/{fileName}".Replace("\\", "/");
        if (File.Exists(destAssetPath))
        {
            Debug.LogWarning($"Asset already exists at: {destAssetPath}");
            return destAssetPath;
        }
        destAssetPath = AssetDatabase.GenerateUniqueAssetPath(destAssetPath);

        string destAbs = Path.GetFullPath(Path.Combine(projectRootAbs, destAssetPath));
        File.Copy(pickedAbsolutePath, destAbs, overwrite: false);

        // Return the Assets/ path so AssetDatabase can import it
        return destAssetPath;
    }
}
