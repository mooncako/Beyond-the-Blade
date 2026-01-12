using System.IO;
using Animancer;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEditor;
using UnityEngine;

public class NewEnemySetup : EditorWindow
{
    private Vector2 _scroll;
    private GameObject _model;
    private string _enemyName = "";
    private PersonalityType _personalityType;
    private Vector3 _attackPointOffset = new Vector3(0, .9f, .25f);
    private EnemyStatsSO _enemyStatsSO;
    private AttackSensorConfigSO _attackSensorConfigSO;
    private StrafeSensorConfigSO _strafeSensorConfigSO;
    private AvailableSkillSO _availableSkillSO;

    void OnGUI()
    {
        GUILayout.Label("ENEMY SETUP TOOL", EditorStyles.whiteLargeLabel);
        var newModel = (GameObject)EditorGUILayout.ObjectField(
            "Enemy Model",
            _model,
            typeof(GameObject),
            false
        );

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        GUILayout.Space(8);
        

        if(newModel != _model)
        {
            _model = newModel;

            if(_model != null && string.IsNullOrWhiteSpace(_enemyName))
            {
                _enemyName = _model.name;
            }
        }

        using (new EditorGUI.DisabledScope(_model == null))
        {
            GUILayout.Label("Basic Info", EditorStyles.boldLabel);
            _enemyName = EditorGUILayout.TextField(
                "Enemy Name",
                _enemyName
            );

            GUILayout.Space(8);
            GUILayout.Label("Detailed Setup", EditorStyles.boldLabel);
            _personalityType = (PersonalityType)EditorGUILayout.EnumPopup(
                "Personality Type",
                _personalityType
            );
            _attackPointOffset = EditorGUILayout.Vector3Field(
                "Attack Point Offset",
                _attackPointOffset
            );

#region Enemy Stats
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _enemyStatsSO = (EnemyStatsSO)EditorGUILayout.ObjectField(
                        "Enemy Stats SO",
                        _enemyStatsSO,
                        typeof(EnemyStatsSO),
                        false
                    );

                    using (new EditorGUI.DisabledScope(_enemyStatsSO == null))
                    {
                        if (GUILayout.Button("Edit", GUILayout.MaxWidth(80)))
                        {
                            SOEditor.Open(_enemyStatsSO);
                        }
                    }
                }

                using (new EditorGUI.DisabledScope(_enemyStatsSO != null))
                {
                    if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                    {
                        const string statsFolder = "Assets/ScriptableObjects/Stats/Enemies";
                        string newStatsFolder = $"{statsFolder}/{_enemyName}";
                        bool createdThisCall = false;

                        if (!AssetDatabase.IsValidFolder(newStatsFolder))
                        {
                            Directory.CreateDirectory(newStatsFolder);
                            AssetDatabase.Refresh();
                            createdThisCall = true;
                        }

                        string path = EditorUtility.SaveFilePanelInProject(
                            "Save Enemy Stats",
                            _enemyName,
                            "asset",
                            "Specify where to save the new Enemy Stats.",
                            newStatsFolder
                        );

                        if (string.IsNullOrWhiteSpace(path))
                        {
                            if (createdThisCall)
                            {
                                AssetDatabase.DeleteAsset(newStatsFolder);
                                AssetDatabase.Refresh();
                            }
                            return;
                        }

                        EnemyStatsSO newStatsSO = CreateInstance<EnemyStatsSO>();
                        AssetDatabase.CreateAsset(newStatsSO, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        _enemyStatsSO = newStatsSO;

                    }
                }

            }

            
#endregion
#region AI Configs
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _attackSensorConfigSO = (AttackSensorConfigSO)EditorGUILayout.ObjectField(
                        "Attack Sensor Config",
                        _attackSensorConfigSO,
                        typeof(AttackSensorConfigSO),
                        false
                    );

                    using (new EditorGUI.DisabledScope(_attackSensorConfigSO == null))
                    {
                        if (GUILayout.Button("Edit", GUILayout.MaxWidth(80)))
                        {
                            SOEditor.Open(_attackSensorConfigSO);
                        }
                    }
                }
                

                if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                {
                    const string configsFolder = "Assets/ScriptableObjects/AI/Configs";
                    string newConfigsFolder = $"{configsFolder}/{_enemyName}";
                    bool createdThisCall = false;

                    if(!AssetDatabase.IsValidFolder(newConfigsFolder))
                    {
                        Directory.CreateDirectory(newConfigsFolder);
                        AssetDatabase.Refresh();
                        createdThisCall = true;
                    }

                    string path = EditorUtility.SaveFilePanelInProject(
                        "Save Attack Sensor Config",
                        $"{_enemyName}AttackSensorConfig",
                        "asset",
                        "Specify where to save the new Attack Sensor Config.",
                        newConfigsFolder
                    );

                    if(string.IsNullOrWhiteSpace(path))
                    {
                        if(createdThisCall)
                        {
                            AssetDatabase.DeleteAsset(newConfigsFolder);
                            AssetDatabase.Refresh();
                        }
                        return;
                    }

                    AttackSensorConfigSO newAttackSensorConfigSO = CreateInstance<AttackSensorConfigSO>();
                    AssetDatabase.CreateAsset(newAttackSensorConfigSO, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _attackSensorConfigSO = newAttackSensorConfigSO;
                    
                }
            }

            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _strafeSensorConfigSO = (StrafeSensorConfigSO)EditorGUILayout.ObjectField(
                        "Strafe Sensor Config",
                        _strafeSensorConfigSO,
                        typeof(StrafeSensorConfigSO),
                        false
                    );

                    using (new EditorGUI.DisabledScope(_strafeSensorConfigSO == null))
                    {
                        if (GUILayout.Button("Edit", GUILayout.MaxWidth(80)))
                        {
                            SOEditor.Open(_strafeSensorConfigSO);
                        }
                    }
                }
                

                if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                {
                    const string configsFolder = "Assets/ScriptableObjects/AI/Configs";
                    string newConfigsFolder = $"{configsFolder}/{_enemyName}";
                    bool createdThisCall = false;

                    if(!AssetDatabase.IsValidFolder(newConfigsFolder))
                    {
                        Directory.CreateDirectory(newConfigsFolder);
                        AssetDatabase.Refresh();
                        createdThisCall = true;
                    }

                    string path = EditorUtility.SaveFilePanelInProject(
                        "Save Strafe Sensor Config",
                        $"{_enemyName}StrafeSensorConfig",
                        "asset",
                        "Specify where to save the new Strafe Sensor Config.",
                        newConfigsFolder
                    );

                    if(string.IsNullOrWhiteSpace(path))
                    {
                        if(createdThisCall)
                        {
                            AssetDatabase.DeleteAsset(newConfigsFolder);
                            AssetDatabase.Refresh();
                        }
                        return;
                    }

                    StrafeSensorConfigSO newStrafeSensorConfigSO = CreateInstance<StrafeSensorConfigSO>();
                    AssetDatabase.CreateAsset(newStrafeSensorConfigSO, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _strafeSensorConfigSO = newStrafeSensorConfigSO;
                    
                }
            }
#endregion
#region Skills
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _availableSkillSO = (AvailableSkillSO)EditorGUILayout.ObjectField(
                        "Available Skill SO",
                        _availableSkillSO,
                        typeof(AvailableSkillSO),
                        false
                    );

                    using (new EditorGUI.DisabledScope(_availableSkillSO == null))
                    {
                        if (GUILayout.Button("Edit", GUILayout.MaxWidth(80)))
                        {
                            SOEditor.Open(_availableSkillSO);
                        }
                    }
                }
                

                if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                {
                    const string skillsFolder = "Assets/ScriptableObjects/Weapon/EnemyWeapons";
                    string newSkillsFolder = $"{skillsFolder}";
                    bool createdThisCall = false;

                    if(!AssetDatabase.IsValidFolder(newSkillsFolder))
                    {
                        Directory.CreateDirectory(newSkillsFolder);
                        AssetDatabase.Refresh();
                        createdThisCall = true;
                    }

                    string path = EditorUtility.SaveFilePanelInProject(
                        "Save Available Skill SO",
                        $"{_enemyName}Weapon",
                        "asset",
                        "Specify where to save the new Available Skill SO.",
                        newSkillsFolder
                    );

                    if(string.IsNullOrWhiteSpace(path))
                    {
                        if(createdThisCall)
                        {
                            AssetDatabase.DeleteAsset(newSkillsFolder);
                            AssetDatabase.Refresh();
                        }
                        return;
                    }

                    AvailableSkillSO newAvailableSkillSO = CreateInstance<AvailableSkillSO>();
                    AssetDatabase.CreateAsset(newAvailableSkillSO, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _availableSkillSO = newAvailableSkillSO;
                    
                }
            }
#endregion
            
        }


        if (_model == null)
        {
            EditorGUILayout.HelpBox("Assign an enemy model to start setup.", MessageType.Info);
        }

        
        GUILayout.Space(8);

        if (GUILayout.Button("Setup New Enemy"))
        {
            CreateEnemy();
        }

        EditorGUILayout.EndScrollView();
    }
#region Helper Functions
    private void CreateEnemy()
    {
        if (_model == null)
        {
            EditorUtility.DisplayDialog("Missing Model", "Please assign a model for the enemy.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(_enemyName))
        {
            EditorUtility.DisplayDialog("Missing Name", "Please enter a name for the enemy.", "OK");
            return;
        }

        const string prefabFolder = "Assets/Prefabs/Enemies";
        if(!AssetDatabase.IsValidFolder(prefabFolder))
        {
            Directory.CreateDirectory(prefabFolder);
            AssetDatabase.Refresh();
        }
 
        GameObject root = new GameObject(_enemyName);
        Undo.RegisterCreatedObjectUndo(root, "Create Enemy (Temp)");

        AddComponents(root); // Adding required components

        GameObject modelinstance = (GameObject)PrefabUtility.InstantiatePrefab(_model);
        Undo.RegisterCreatedObjectUndo(modelinstance, "Create Enemy Model (Temp)");
        modelinstance.name = "Model";
        modelinstance.transform.SetParent(root.transform, false);
        modelinstance.transform.localPosition = Vector3.zero;
        modelinstance.transform.localRotation = Quaternion.identity;
        modelinstance.transform.localScale = Vector3.one;

        string path = $"{prefabFolder}/{_enemyName}.prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(root, path);

        DestroyImmediate(root);

        Selection.activeGameObject = prefabAsset;
        EditorGUIUtility.PingObject(prefabAsset);
        EditorUtility.DisplayDialog("Success", "New enemy setup complete!", "OK");
    }

    private void AddComponents(GameObject root)
    {
        if(root.GetComponent<EnemyController>() == null)
        {
            root.AddComponent<EnemyController>();
        }
        if(root.GetComponent<Vision>() != null)
        {
            root.GetComponent<Vision>().SetupMasks(
                LayerMask.GetMask("Player", "Corpse"),
                LayerMask.GetMask("Character", "Corpse")
            );
        }
        if(root.GetComponent<AnimancerComponent>() != null)
        {
            root.GetComponent<AnimancerComponent>().Animator = root.GetComponent<Animator>();
        }
        if(root.GetComponent<Posture>() == null)
        {
            root.AddComponent<Posture>();
        }
        if(root.GetComponent<Energy>() == null)
        {
            root.AddComponent<Energy>();
        }
        if(root.GetComponent<AnimationStateMachine>() == null)
        {
            root.AddComponent<AnimationStateMachine>();
        }
        if(root.GetComponent<RedirectRootMotionToRigidbody>() == null)
        {
            root.AddComponent<RedirectRootMotionToRigidbody>();
        }
        if(root.GetComponent<DependencyInjector>() == null)
        {
            root.AddComponent<DependencyInjector>();
        }
        if(root.GetComponent<ProactiveControllerBehaviour>() == null)
        {
            root.AddComponent<ProactiveControllerBehaviour>();
        }
        if(root.GetComponent<GoapActionProvider>() == null)
        {
            root.AddComponent<GoapActionProvider>();
        }
        if(root.GetComponent<GoapBehaviour>() == null)
        {
            GoapBehaviour behavior = root.AddComponent<GoapBehaviour>();
            DependencyInjector injector = root.GetComponent<DependencyInjector>();
            behavior.configInitializer = injector;
        }
        if(root.GetComponent<AgentBehaviour>() == null)
        {
            AgentBehaviour agentBehaviour = root.AddComponent<AgentBehaviour>();
            GoapActionProvider actionProvider = root.GetComponent<GoapActionProvider>();
            agentBehaviour.ActionProviderBase = actionProvider;
        }
        if(root.GetComponent<AgentMoveBehavior>() == null)
        {
            root.AddComponent<AgentMoveBehavior>();
        }
    }


    [MenuItem("Tools/New Enemy Setup", false, 10)]
    private static void SetupNewEnemy()
    {

        GetWindow(typeof(NewEnemySetup), false, "New Enemy Setup");
    }
#endregion
}
