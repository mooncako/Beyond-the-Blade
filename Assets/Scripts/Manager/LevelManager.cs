using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableShopLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableRecoveryLevelPrefabs;
    [SerializeField, BoxGroup("Debug")] private BiomeType _currentBiome;
    [SerializeField, BoxGroup("Debug")] public LevelType CurrentLeveltype;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _currentLevel;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<LevelType> _exitsLevelType = new List<LevelType>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        CurrentLevelIndex++;

        // Choose the current level based on CurrentLeveltype and biome
        SelectLevel();

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

    private void SelectLevel()
    {
        switch (CurrentLeveltype)
        {
            case LevelType.Reguler:
                _currentLevel = SelectLevel(_availableNormalLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Recover:
                _currentLevel = SelectLevel(_availableRecoveryLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Shop:
                _currentLevel = SelectLevel(_availableShopLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
        }
    }

    private LevelSystem SelectLevel(LevelSystem[] levels)
    {
        List<LevelSystem> possibleLevels = new List<LevelSystem>();

        for (int i = 0; i <= levels.Length; i++)
        {
            if (levels[i].BiomeType == _currentBiome)
            {
                possibleLevels.Add(levels[i]);
            }
        }

        return possibleLevels[Random.Range(0, possibleLevels.Count)];
    }
}
