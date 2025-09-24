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
    [SerializeField, BoxGroup("References")] private GameObject _gatePrefab;
    [SerializeField, BoxGroup("References")] private PickupFactory _pickupFactory;
    [SerializeField, BoxGroup("Settings")] private BiomeType _defaultBiome;
    [SerializeField, BoxGroup("Settings")] private LevelType _defaultLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private BiomeType _currentBiome;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public LevelType CurrentLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _currentLevel;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<LevelType> _exitsLevelType = new List<LevelType>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _doOnce = true;

    [SerializeField, HideInInspector] private bool _isSetupComplete = false;
    private LevelSystem _selectedSystemPrefab;
    

    void OnValidate()
    {
        if (_pickupFactory == null) _pickupFactory = GetComponent<PickupFactory>();
    }

    protected override void Awake()
    {
        base.Awake();

        
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
        if (scene.name == "TestMainMenu" || scene.name == "TestHub")
        {
            ResetManager();
        }


        if (_doOnce)
        {
            _doOnce = false;
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
                _selectedSystemPrefab = PickPossibleLevel(_availableNormalLevelPrefabs);
                _currentLevel = Instantiate(_selectedSystemPrefab, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Recover:
                _selectedSystemPrefab = PickPossibleLevel(_availableRecoveryLevelPrefabs);
                _currentLevel = Instantiate(_selectedSystemPrefab, Vector3.zero, Quaternion.identity);
                break;
            case LevelType.Shop:
                _selectedSystemPrefab = PickPossibleLevel(_availableShopLevelPrefabs);
                _currentLevel = Instantiate(_selectedSystemPrefab, Vector3.zero, Quaternion.identity);
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
        _doOnce = true;
        switch (CurrentLevelType)
        {
            case LevelType.Reguler:
                _pickupFactory.SpawnPickup(_currentLevel.PickupSpawnPosition.position);
                break;
        }
        //TODO: Spawn exits

        for (int i = 0; i < _currentLevel.ExitPosList.Count; i++)
        {
            Gate gate = Instantiate(_gatePrefab, _currentLevel.ExitPosList[i].transform.position, _currentLevel.ExitPosList[i].transform.rotation).GetComponentInChildren<Gate>();
            gate.SetLevelName(SceneManager.GetActiveScene().name);
        }
    }
    
}
