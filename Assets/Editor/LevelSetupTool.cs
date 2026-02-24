using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtils;

public class LevelSetupTool : EditorWindow
{
    private GameObject _groundMesh;
    private string _levelName;
    private BiomeType _biomeType;
    private string _prefabFolder;

    [MenuItem("Tools/Level Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<LevelSetupTool>("Level Setup Tool");
    }

    private void OnGUI()
    {
        _groundMesh = (GameObject)EditorGUILayout.ObjectField("Ground Mesh", _groundMesh, typeof(GameObject), true);
        EditorGUILayout.LabelField("Level Name");
        _levelName = EditorGUILayout.TextField("", _levelName);
        EditorGUILayout.LabelField("Biome");
        _biomeType = (BiomeType)EditorGUILayout.EnumPopup(_biomeType);
        EditorGUILayout.LabelField("Prefab Folder");
        switch(_biomeType)
        {
            case BiomeType.City:
                _prefabFolder = "Assets/Prefabs/Level/LevelPrefabs/City";
                break;
            case BiomeType.Market:
                _prefabFolder = "Assets/Prefabs/Level/LevelPrefabs/Market";
                break;
            case BiomeType.Shrine:
                _prefabFolder = "Assets/Prefabs/Level/LevelPrefabs/Shrine";
                break;
        }

        _prefabFolder = EditorGUILayout.TextField("", _prefabFolder);

        using(new EditorGUI.DisabledScope(!IsMeshValid(_groundMesh) || _levelName.IsNullOrWhiteSpace()))
        {
            if (GUILayout.Button("Setup Level"))
            {
                SetupLevel(_groundMesh, _levelName);
            }
        }

        if (!IsMeshValid(_groundMesh))
        {
            EditorGUILayout.HelpBox("Assign a valid ground mesh to start setup. (Requires proper mesh filter and mesh)", MessageType.Info);
        }

        if(_levelName.IsNullOrWhiteSpace())
        {
            EditorGUILayout.HelpBox("Please enter a valid level name.", MessageType.Info);
        }
        
    }

    private void SetupLevel(GameObject groundMesh, string levelName)
    {
        if(groundMesh.GetComponent<GroundMesh>() == null)
        {
            groundMesh.AddComponent<GroundMesh>();
        }
        GameObject go = new GameObject(levelName);
        groundMesh.transform.SetParent(go.transform);
        groundMesh.transform.position = Vector3.zero;
        groundMesh.layer = LayerMask.NameToLayer("Ground");
        groundMesh.name = "GroundMesh";
        
        GameObject spawnPosObject = new GameObject("PlayerSpawnPosition");
        spawnPosObject.AddComponent<SpawnPos>();
        spawnPosObject.transform.SetParent(go.transform);

        GameObject pickupSpawnPosObject = new GameObject("PickupSpawnPosition");
        pickupSpawnPosObject.AddComponent<PickupSpawnPos>();
        pickupSpawnPosObject.transform.SetParent(go.transform);

        GameObject environmentProps = new GameObject("EnvironmentProps");
        environmentProps.layer = LayerMask.NameToLayer("Environment");
        environmentProps.transform.SetParent(go.transform);

        GameObject GameplayProps = new GameObject("GameplayProps");
        GameplayProps.transform.SetParent(go.transform);

        LevelSystem levelSystem = go.AddComponent<LevelSystem>();

        GameObject levelAssignerObject = new GameObject("LevelAssigner");
        levelAssignerObject.AddComponent<LevelAssigner>();
        levelAssignerObject.transform.SetParent(go.transform);

        levelSystem.AssignReferences();

        if (!AssetDatabase.IsValidFolder(_prefabFolder))
        {
            Directory.CreateDirectory(_prefabFolder);
            AssetDatabase.Refresh();
        }

        string mainPrefabPath = $"{_prefabFolder}/{_levelName}.prefab";
        mainPrefabPath = AssetDatabase.GenerateUniqueAssetPath(mainPrefabPath);

        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(go, mainPrefabPath);

        DestroyImmediate(go);

        Selection.activeGameObject = prefabAsset;
        EditorGUIUtility.PingObject(prefabAsset);

    }

    private bool IsMeshValid(GameObject groundMesh)
    {
        if(groundMesh == null)
        {
            return false;
        }

        MeshFilter meshFilter = groundMesh.GetComponent<MeshFilter>();
        if(meshFilter == null || meshFilter.sharedMesh == null)
        {
            meshFilter = groundMesh.GetComponentInChildren<MeshFilter>();
            if(meshFilter == null || meshFilter.sharedMesh == null)
                return false;
        }
        return true;
    }
}
