using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>,
    MMEventListener<LevelRandomizeCompleteEvent>,
    MMEventListener<RoomClearedEvent>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableShopLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableRecoveryLevelPrefabs;
    [SerializeField, BoxGroup("References")] private PickupFactory _pickupFactory;
    [SerializeField, BoxGroup("Settings")] private BiomeType _defaultBiome;
    [SerializeField, BoxGroup("Settings")] private LevelType _defaultLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private BiomeType _currentBiome;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public LevelType CurrentLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _currentLevel;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<LevelType> _exitsLevelType = new List<LevelType>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;

    [SerializeField, HideInInspector] private bool _isSetupComplete = false;

    void OnValidate()
    {
        if (_pickupFactory == null) _pickupFactory = GetComponent<PickupFactory>();
    }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        CurrentLevelIndex++;

        if (!_isSetupComplete)
        {
            _currentBiome = _defaultBiome;
            CurrentLevelType = _defaultLevelType;
            _isSetupComplete = true;
        }

        // Choose the current level based on CurrentLeveltype and biome
        SelectLevel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.MMEventStartListening<LevelRandomizeCompleteEvent>();
        this.MMEventStartListening<RoomClearedEvent>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
        this.MMEventStopListening<RoomClearedEvent>();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
        this.MMEventStopListening<RoomClearedEvent>();
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
        switch (CurrentLevelType)
        {
            case LevelType.Reguler:
                _currentLevel = PickPossibleLevel(_availableNormalLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Recover:
                _currentLevel = PickPossibleLevel(_availableRecoveryLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Shop:
                _currentLevel = PickPossibleLevel(_availableShopLevelPrefabs);
                Instantiate(_currentLevel, Vector3.zero, Quaternion.identity);
                break;
        }
    }

    private LevelSystem PickPossibleLevel(LevelSystem[] levels)
    {
        List<LevelSystem> possibleLevels = new List<LevelSystem>();

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].BiomeType == _currentBiome)
            {
                possibleLevels.Add(levels[i]);
            }
        }

        return possibleLevels[Random.Range(0, possibleLevels.Count)];
    }

    private void CalculateExitTypes()
    {
        float possibilityIndex;
        _exitsLevelType.Clear();
        for (int i = 0; i < _currentLevel.ExitPosList.Count; i++)
        {
            possibilityIndex = Random.Range(0, 1);
            if (possibilityIndex <= _currentLevel.ExitsProbability.RegularExitPercentage)
            {
                _exitsLevelType.Add(LevelType.Reguler);
            }
            else if (possibilityIndex <= _currentLevel.ExitsProbability.RegularExitPercentage + _currentLevel.ExitsProbability.RecoveryExitPercentage)
            {
                _exitsLevelType.Add(LevelType.Recover);
            }
            else
            {
                _exitsLevelType.Add(LevelType.Shop);
            }
        }
    }

    public void OnMMEvent(LevelRandomizeCompleteEvent e)
    {
        if (e.State == EventStateType.OnEventEnd)
        {
            CalculateExitTypes();
        }
    }

    public void OnMMEvent(RoomClearedEvent e)
    {
        _pickupFactory.SpawnPickup(_currentLevel.PickupSpawnPosition.position);
    }
    
}
