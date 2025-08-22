using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableShopLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableReceoveryLevelPrefabs;
    [SerializeField, BoxGroup("Debug")] private BiomeType _currentBiome;
    [SerializeField, BoxGroup("Debug")] public LevelType CurrentLeveltype;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<LevelType> _exitsLevelType = new List<LevelType>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        CurrentLevelIndex++;

        // Choose the current level based

        // Choose the exit level type
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "MainMenu")
        {
            ResetManager();
        }
    }

    private void ResetManager()
    {
        CurrentLevelIndex = 0;
    }
}
