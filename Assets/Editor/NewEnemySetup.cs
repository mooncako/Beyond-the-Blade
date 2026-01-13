using System;
using System.IO;
using Animancer;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public class NewEnemySetup : EditorWindow
{
    private Vector2 _scroll;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _attackPoint;
    [SerializeField] private string _enemyName = "";
    [SerializeField] private PersonalityType _personalityType;
    [SerializeField] private Vector3 _attackPointOffset = new Vector3(0, .9f, .25f);
    [SerializeField] private Vector3 _playerSensorOffset = new Vector3(0, .9f, 0f);
    [SerializeField] private Vector3 _parryColliderOffset = new Vector3(0, .9f, 0f);
    [SerializeField] private Vector3 _characterUIOffset = new Vector3(0, 2.4f, 0f);
    [SerializeField] private EnemyStatsSO _enemyStatsSO;
    [SerializeField] private AttackSensorConfigSO _attackSensorConfigSO;
    [SerializeField] private StrafeSensorConfigSO _strafeSensorConfigSO;
    [SerializeField] private AvailableSkillSO _availableSkillSO;
    [SerializeField] private ModifierDatabaseSO _modifierDatabaseSO;
    [SerializeField] private SkillAnimationDatabaseSO _skillAnimationDatabaseSO;
    [SerializeField] private EnemySkillsSO _enemySkillsSO;

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


        if (newModel != _model)
        {
            _model = newModel;

            if (_model != null && string.IsNullOrWhiteSpace(_enemyName))
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
            _playerSensorOffset = EditorGUILayout.Vector3Field(
                "Player Sensor Offset",
                _playerSensorOffset
            );
            _parryColliderOffset = EditorGUILayout.Vector3Field(
                "Parry Collider Offset",
                _parryColliderOffset
            );
            _characterUIOffset = EditorGUILayout.Vector3Field(
                "Character UI Offset",
                _characterUIOffset
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

                using (new EditorGUI.DisabledScope(_attackSensorConfigSO != null))
                {
                    if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                    {
                        const string configsFolder = "Assets/ScriptableObjects/AI/Configs";
                        string newConfigsFolder = $"{configsFolder}/{_enemyName}";
                        bool createdThisCall = false;

                        if (!AssetDatabase.IsValidFolder(newConfigsFolder))
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

                        if (string.IsNullOrWhiteSpace(path))
                        {
                            if (createdThisCall)
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

                using(new EditorGUI.DisabledScope(_strafeSensorConfigSO != null))
                {
                    if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                    {
                        const string configsFolder = "Assets/ScriptableObjects/AI/Configs";
                        string newConfigsFolder = $"{configsFolder}/{_enemyName}";
                        bool createdThisCall = false;

                        if (!AssetDatabase.IsValidFolder(newConfigsFolder))
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

                        if (string.IsNullOrWhiteSpace(path))
                        {
                            if (createdThisCall)
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

                using (new EditorGUI.DisabledScope(_availableSkillSO != null))
                {
                    if (GUILayout.Button("New", GUILayout.MaxWidth(80)))
                    {
                        const string skillsFolder = "Assets/ScriptableObjects/Weapon/EnemyWeapons";
                        string newSkillsFolder = $"{skillsFolder}";
                        bool createdThisCall = false;

                        if (!AssetDatabase.IsValidFolder(newSkillsFolder))
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

                        if (string.IsNullOrWhiteSpace(path))
                        {
                            if (createdThisCall)
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
                
            }

            GUILayout.Space(8);

            _modifierDatabaseSO = (ModifierDatabaseSO)AssetDatabase.LoadAssetAtPath(
                "Assets/ScriptableObjects/Database/SkillModifierDatabase.asset",
                typeof(ModifierDatabaseSO)
            );

            EditorGUILayout.ObjectField(
                "Modifier Database",
                _modifierDatabaseSO,
                typeof(ModifierDatabaseSO),
                false
            );

            _skillAnimationDatabaseSO = (SkillAnimationDatabaseSO)AssetDatabase.LoadAssetAtPath(
                "Assets/ScriptableObjects/Database/SkillAnimationDatabase.asset",
                typeof(SkillAnimationDatabaseSO)
            );

            EditorGUILayout.ObjectField(
                "Skill Animation Database",
                _skillAnimationDatabaseSO,
                typeof(SkillAnimationDatabaseSO),
                false
            );

            _enemySkillsSO = (EnemySkillsSO)AssetDatabase.LoadAssetAtPath(
                "Assets/ScriptableObjects/Database/EnemySkillsDatabase.asset",
                typeof(EnemySkillsSO)
            );

            EditorGUILayout.ObjectField(
                "Enemy Skills Database",
                _enemySkillsSO,
                typeof(EnemySkillsSO),
                false
            );

            #endregion

        }


        if (_model == null)
        {
            EditorGUILayout.HelpBox("Assign an enemy model to start setup.", MessageType.Info);
        }

        if (_modifierDatabaseSO == null)
        {
            EditorGUILayout.HelpBox("Can't find modifier database at 'Assets/ScriptableObjects/Database/SkillModifierDatabase.asset'", MessageType.Error);
        }


        GUILayout.Space(8);

        if (GUILayout.Button("Generate Required Scripts"))
        {
            DerivedComponentGenerator.CreateDerivedScriptOnly(typeof(CapabilityFactory), $"{_enemyName}CapabilityFactory", "Assets/Scripts/GOAP/Factory/", null,
$@"
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class {_enemyName}CapabilityFactory : CapabilityFactory
{{
    public override ICapabilityConfig Create()
    {{
        var builder = new CapabilityBuilder(""{_enemyName}"");

        BuildGoals(builder);
        BuildActions(builder);
        BuildSensors(builder);

        return builder.Build();
    }}

    protected override void BuildGoals(CapabilityBuilder builder)
    {{
        base.BuildGoals(builder);
    }}

    protected override void BuildActions(CapabilityBuilder builder)
    {{
        base.BuildActions(builder);

        // Add custom actions here

    }}

    protected override void BuildSensors(CapabilityBuilder builder)
    {{
        base.BuildSensors(builder);
    }}
}}
");

            DerivedComponentGenerator.CreateDerivedScriptOnly(typeof(AgentTypeFactoryBase), $"{_enemyName}Agent", "Assets/Scripts/GOAP/Agent", null,
    $@"
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

public class {_enemyName}Agent : AgentTypeFactoryBase
{{
    public override IAgentTypeConfig Create()
    {{
        var factory = new AgentTypeBuilder(""{_enemyName}"");
        factory.AddCapability<{_enemyName}CapabilityFactory>();
        return factory.Build();
    }}
}}
");
            DerivedComponentGenerator.CreateDerivedScriptOnly(typeof(Brain), $"{_enemyName}Brain", "Assets/Scripts/GOAP/Brain", null,
    $@"
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.GenTest;
using UnityEngine;

public class {_enemyName}Brain : Brain
{{
    protected override void OnValidate()
    {{
        base.OnValidate();
    }}

    protected override void OnEnable()
    {{
        base.OnEnable();
    }}

    protected override void OnDisable()
    {{
        base.OnDisable();
    }}

    protected override void Awake()
    {{
        if(_provider != null && _goap != null)
        {{
            if(_provider.AgentTypeBehaviour == null)
            {{
                _provider.AgentType = _goap.GetAgentType(""{_enemyName}"");
            }}
        }}
    }}

    protected override void Start()
    {{
        base.Start();
        _provider.RequestGoal<ChasePlayerGoal>(false);
    }}

    protected override void OnActionEnd(IAction action)
    {{
        if (!gameObject.activeSelf) return;
        if(_isPlayerInCombatRange)
        {{
            switch (Personality)
            {{
                case PersonalityType.Aggressive:
                    _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
                    break;
                case PersonalityType.Cautious:
                    _provider.RequestGoal<KillPlayerCautiousGoal, StrafeGoal>();
                    break;
                case PersonalityType.Evasive:
                    _provider.RequestGoal<KillPlayerGoal, StrafeGoal>(); // TODO: Add evasive goal
                    break;
            }}
        }}
        else
        {{
            _provider.RequestGoal<ChasePlayerGoal>();
        }}
    }}

    protected override void OnPlayerEnterCombatRange(Transform player)
    {{
        _provider.ClearGoal();
        switch(Personality)
        {{
            case PersonalityType.Aggressive:
                _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
                break;
            case PersonalityType.Cautious:
                _provider.RequestGoal<KillPlayerCautiousGoal, StrafeGoal>();
                break;
            case PersonalityType.Evasive:
                _provider.RequestGoal<KillPlayerGoal, StrafeGoal>(); // TODO: Add evasive goal
                break;
        }}
        
        _isPlayerInCombatRange = true;
    }}

    protected override void OnPlayerExitCombatRange(Vector3 lastKnownPosition)
    {{
        _provider.ClearGoal();
        _provider.RequestGoal<ChasePlayerGoal>();
        _isPlayerInCombatRange = false;
    }}
}}
        "
            );

            CompilationPipeline.RequestScriptCompilation();

        }


        if (GUILayout.Button("Setup New Enemy"))
        {
            CreateEnemy();
        }


        EditorGUILayout.EndScrollView();
    }
    #region Creation Logic
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

        if (_attackSensorConfigSO == null || _availableSkillSO == null || _enemyStatsSO == null || _strafeSensorConfigSO == null || _modifierDatabaseSO == null)
        {
            EditorUtility.DisplayDialog("Incomplete Setup", "Please ensure all required Scriptable Objects are assigned.", "OK");
            return;
        }

        if (TypeFinder.FindType($"{_enemyName}CapabilityFactory") == null ||
            TypeFinder.FindType($"{_enemyName}Agent") == null ||
            TypeFinder.FindType($"{_enemyName}Brain") == null)
        {
            EditorUtility.DisplayDialog("Missing Scripts", "Please generate the required scripts before setting up the enemy.", "OK");
            return;
        }

        const string prefabFolder = "Assets/Prefabs/Enemies";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            Directory.CreateDirectory(prefabFolder);
            AssetDatabase.Refresh();
        }

        GameObject root = new GameObject(_enemyName);
        Undo.RegisterCreatedObjectUndo(root, "Create Enemy (Temp)");

        #region Setup Child
        GameObject modelinstance = (GameObject)PrefabUtility.InstantiatePrefab(_model);
        Undo.RegisterCreatedObjectUndo(modelinstance, "Create Enemy Model (Temp)");
        modelinstance.name = "Model";
        modelinstance.transform.SetParent(root.transform, false);
        modelinstance.transform.localPosition = Vector3.zero;
        modelinstance.transform.localRotation = Quaternion.identity;
        modelinstance.transform.localScale = Vector3.one;


        GameObject agent = new GameObject("Agent");
        Undo.RegisterCreatedObjectUndo(agent, "Create Enemy Agent (Temp)");
        agent.transform.SetParent(root.transform, false);
        agent.transform.localPosition = Vector3.zero;
        agent.transform.localRotation = Quaternion.identity;
        agent.transform.localScale = Vector3.one;
        agent.AddComponent(TypeFinder.FindType($"{_enemyName}Agent"));




        GameObject playerSensor = new GameObject("PlayerSensor");
        Undo.RegisterCreatedObjectUndo(playerSensor, "Create Enemy PlayerSensor (Temp)");
        playerSensor.transform.SetParent(root.transform, false);
        playerSensor.transform.localPosition = _playerSensorOffset;
        playerSensor.transform.localRotation = Quaternion.identity;
        playerSensor.transform.localScale = Vector3.one;
        playerSensor.AddComponent<PlayerSensor>();

        _attackPoint = new GameObject("AttackPoint");
        Undo.RegisterCreatedObjectUndo(_attackPoint, "Create Enemy AttackPoint (Temp)");
        _attackPoint.transform.SetParent(root.transform, false);
        _attackPoint.transform.localPosition = _attackPointOffset;
        _attackPoint.transform.localRotation = Quaternion.identity;
        _attackPoint.transform.localScale = Vector3.one;

        GameObject parryColliderPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
            "Assets/Prefabs/Combat/ParryCollider.prefab",
            typeof(GameObject)
        );
        GameObject parryColliderInstance = (GameObject)PrefabUtility.InstantiatePrefab(parryColliderPrefab);
        Undo.RegisterCreatedObjectUndo(parryColliderInstance, "Create Enemy ParryCollider (Temp)");
        parryColliderInstance.name = "ParryCollider";
        parryColliderInstance.transform.SetParent(root.transform, false);
        parryColliderInstance.transform.localPosition = _attackPointOffset;
        parryColliderInstance.transform.localRotation = Quaternion.identity;
        parryColliderInstance.transform.localScale = Vector3.one;

        #endregion

        AddRootComponents(root); // Adding required components

        GameObject weaponObj = new GameObject($"Enemy{_enemyName}Weapon");
        Undo.RegisterCreatedObjectUndo(weaponObj, "Create Enemy Weapon (Temp)");
        Weapon weapon = weaponObj.AddComponent<Weapon>();
        weapon.AssignDatabase(_skillAnimationDatabaseSO, _enemySkillsSO, _availableSkillSO);
        string weaponPrefabPath = $"Assets/Prefabs/Weapon/Enemy{_enemyName}Weapon.prefab";
        weaponPrefabPath = AssetDatabase.GenerateUniqueAssetPath(weaponPrefabPath);
        GameObject weaponPrefab = PrefabUtility.SaveAsPrefabAsset(weaponObj, weaponPrefabPath);
        DestroyImmediate(weaponObj);
        GameObject weaponInstance = (GameObject)PrefabUtility.InstantiatePrefab(weaponPrefab);
        Undo.RegisterCreatedObjectUndo(weaponInstance, "Create Enemy Weapon Instance (Temp)");
        weaponInstance.name = $"Enemy{_enemyName}Weapon";
        weaponInstance.transform.SetParent(root.transform, false);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;
        weaponInstance.transform.localScale = Vector3.one;

        GameObject characterUIPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
            "Assets/Prefabs/UI/CharacterUI.prefab",
            typeof(GameObject)
        );
        GameObject characterUIInstance = (GameObject)PrefabUtility.InstantiatePrefab(characterUIPrefab);
        Undo.RegisterCreatedObjectUndo(characterUIInstance, "Create Enemy CharacterUI (Temp)");
        characterUIInstance.name = "CharacterUI";
        characterUIInstance.transform.SetParent(root.transform, false);
        characterUIInstance.transform.localPosition = _characterUIOffset;
        characterUIInstance.transform.localRotation = Quaternion.identity;
        characterUIInstance.transform.localScale = Vector3.one;

        GameObject floatingTextPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
            "Assets/Prefabs/UI/FloatingText.prefab",
            typeof(GameObject)
        );
        GameObject floatingTextInstance = (GameObject)PrefabUtility.InstantiatePrefab(floatingTextPrefab);
        Undo.RegisterCreatedObjectUndo(floatingTextInstance, "Create Enemy FloatingText (Temp)");
        floatingTextInstance.name = "FloatingText";
        floatingTextInstance.transform.SetParent(root.transform, false);
        floatingTextInstance.transform.localPosition = Vector3.zero;
        floatingTextInstance.transform.localRotation = Quaternion.identity;
        floatingTextInstance.transform.localScale = Vector3.one;

        string mainPrefabPath = $"{prefabFolder}/{_enemyName}.prefab";
        mainPrefabPath = AssetDatabase.GenerateUniqueAssetPath(mainPrefabPath);

        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(root, mainPrefabPath);

        DestroyImmediate(root);

        Selection.activeGameObject = prefabAsset;
        EditorGUIUtility.PingObject(prefabAsset);
        EditorUtility.DisplayDialog("Success", "New enemy setup complete!", "OK");
    }
    #endregion

    #region Helper Methods

    private void AddRootComponents(GameObject root)
    {
        if (root.GetComponent<DependencyInjector>() == null)
        {
            DependencyInjector injector = root.AddComponent<DependencyInjector>();
            injector.AttackSensorConfig = _attackSensorConfigSO;
            injector.StrafeSensorConfig = _strafeSensorConfigSO;
        }
        if (root.GetComponent<ProactiveControllerBehaviour>() == null)
        {
            root.AddComponent<ProactiveControllerBehaviour>();
        }
        if (root.GetComponent<GoapActionProvider>() == null)
        {
            root.AddComponent<GoapActionProvider>();
        }
        if (root.GetComponent<GoapBehaviour>() == null)
        {
            GoapBehaviour behavior = root.AddComponent<GoapBehaviour>();
            DependencyInjector injector = root.GetComponent<DependencyInjector>();
            behavior.configInitializer = injector;
        }
        if (root.GetComponent<AgentBehaviour>() == null)
        {
            AgentBehaviour agentBehaviour = root.AddComponent<AgentBehaviour>();
            GoapActionProvider actionProvider = root.GetComponent<GoapActionProvider>();
            agentBehaviour.ActionProviderBase = actionProvider;
        }
        if (root.GetComponent<AgentMoveBehavior>() == null)
        {
            root.AddComponent<AgentMoveBehavior>();
        }

        if (root.GetComponent<Vision>() != null)
        {
            root.GetComponent<Vision>().SetupMasks(
                LayerMask.GetMask("Player", "Corpse"),
                LayerMask.GetMask("Character", "Corpse")
            );
        }
        if (root.GetComponent<AnimancerComponent>() != null)
        {
            root.GetComponent<AnimancerComponent>().Animator = root.GetComponent<Animator>();
        }
        if (root.GetComponent<Posture>() == null)
        {
            root.AddComponent<Posture>();
        }
        if (root.GetComponent<Energy>() == null)
        {
            root.AddComponent<Energy>();
        }
        if (root.GetComponent<RedirectRootMotionToRigidbody>() == null)
        {
            root.AddComponent<RedirectRootMotionToRigidbody>();
        }

        if (root.GetComponent(TypeFinder.FindType($"{_enemyName}Brain")) == null)
        {
            root.AddComponent(TypeFinder.FindType($"{_enemyName}Brain"));
        }

        var controller = root.GetComponent<EnemyController>();
        if (controller == null) controller = root.AddComponent<EnemyController>();
        controller.AssignStatsSO(_enemyStatsSO);
        controller.AttackPoint = _attackPoint.transform;


        if (root.GetComponent<Brain>() != null)
        {
            root.GetComponent<Brain>().Personality = _personalityType;
            root.GetComponent<Brain>().AssignAttackSensorConfig(_attackSensorConfigSO);
        }
        if (root.GetComponent<AnimationStateMachine>() == null)
        {
            AnimationStateMachine animationStateMachine = root.AddComponent<AnimationStateMachine>();
            animationStateMachine.AssignModifierDatabase(_modifierDatabaseSO);
        }

    }

    private void CreateComponent(GameObject root, Type derivedType, string className, string folder, string @namespace = null, string script = "")
    {
        DerivedComponentGenerator.CreateDerivedAndAddComponent(
            root,
            derivedType,
            className,
            folder,
            @namespace,
            script
        );
    }


    [MenuItem("Tools/New Enemy Setup", false, 10)]
    private static void SetupNewEnemy()
    {

        GetWindow(typeof(NewEnemySetup), false, "New Enemy Setup");
    }
    #endregion
}
