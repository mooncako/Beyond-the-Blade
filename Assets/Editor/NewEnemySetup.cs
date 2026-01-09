using System.IO;
using Animancer;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEditor;
using UnityEngine;

public class NewEnemySetup : EditorWindow
{
    private GameObject _model;
    private string _enemyName = "";
    private PersonalityType _personalityType;

    void OnGUI()
    {
        GUILayout.Label("Base Info", EditorStyles.boldLabel);
        var newModel = (GameObject)EditorGUILayout.ObjectField(
            "Enemy Model",
            _model,
            typeof(GameObject),
            false
        );

        if(newModel != _model)
        {
            _model = newModel;

            if(_model != null && string.IsNullOrWhiteSpace(_enemyName))
            {
                _enemyName = _model.name;
            }
        }

        GUILayout.Space(8);

        using (new EditorGUI.DisabledScope(_model == null))
        {
            _enemyName = EditorGUILayout.TextField(
                "Enemy Name",
                _enemyName
            );
        }

        if (_model == null)
        {
            EditorGUILayout.HelpBox("Assign an enemy model to enable naming/setup.", MessageType.Info);
        }

        
        GUILayout.Space(8);

        if (GUILayout.Button("Setup New Enemy"))
        {
            CreateEnemy();
        }
    }

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

        const string folder = "Assets/Prefabs/Enemies";
        if(!AssetDatabase.IsValidFolder(folder))
        {
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }

        GameObject root = new GameObject(_enemyName);
        Undo.RegisterCreatedObjectUndo(root, "Create Enemy (Temp)");

        // Add components
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

        GameObject modelinstance = (GameObject)PrefabUtility.InstantiatePrefab(_model);
        Undo.RegisterCreatedObjectUndo(modelinstance, "Create Enemy Model (Temp)");
        modelinstance.name = "Model";
        modelinstance.transform.SetParent(root.transform, false);
        modelinstance.transform.localPosition = Vector3.zero;
        modelinstance.transform.localRotation = Quaternion.identity;
        modelinstance.transform.localScale = Vector3.one;

        


        string path = $"{folder}/{_enemyName}.prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(root, path);

        DestroyImmediate(root);

        Selection.activeGameObject = prefabAsset;
        EditorGUIUtility.PingObject(prefabAsset);
        EditorUtility.DisplayDialog("Success", "New enemy setup complete!", "OK");
    }


    [MenuItem("Tools/New Enemy Setup", false, 10)]
    private static void SetupNewEnemy()
    {
        EditorWindow.GetWindow(typeof(NewEnemySetup), false, "New Enemy Setup");
    }
}
