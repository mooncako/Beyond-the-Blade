using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>,
    MMEventListener<LevelRandomizeCompleteEvent>,
    MMEventListener<RoomClearedEvent>,
    MMEventListener<SpawnRewardEvent>,
    MMEventListener<EnterNewLevelEvent>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableShopLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableRecoveryLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _bossLevelPrefabs;
    [SerializeField, BoxGroup("References")] private GameObject _gatePrefab;
    [SerializeField, BoxGroup("References")] private PickupFactory _pickupFactory;
    [SerializeField, BoxGroup("Settings")] private BiomeType _defaultBiome;
    [SerializeField, BoxGroup("Settings")] private LevelType _defaultLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private BiomeType _currentBiome;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public LevelType CurrentLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _currentLevel;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<LevelType> _exitsLevelType = new List<LevelType>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentLevelCount = 0;
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
        this.MMEventStartListening<SpawnRewardEvent>();
        this.MMEventStartListening<EnterNewLevelEvent>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
        this.MMEventStopListening<RoomClearedEvent>();
        this.MMEventStopListening<SpawnRewardEvent>();
        this.MMEventStopListening<EnterNewLevelEvent>();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
        this.MMEventStopListening<RoomClearedEvent>();
        this.MMEventStopListening<SpawnRewardEvent>();
        this.MMEventStopListening<EnterNewLevelEvent>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == SCENENAME.Menu || scene.name == SCENENAME.Hub)
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
        _currentLevelCount = 0;
    }

    private void SelectLevel()
    {

        // switch (CurrentLevelType)
        // {
        //     case LevelType.Reguler:
        //         _selectedSystemPrefab = PickPossibleLevel(_availableNormalLevelPrefabs);
        //         break;
        //     case LevelType.Recover:
        //         _selectedSystemPrefab = PickPossibleLevel(_availableRecoveryLevelPrefabs);
        //         break;
        //     case LevelType.Shop:
        //         _selectedSystemPrefab = PickPossibleLevel(_availableShopLevelPrefabs);
        //         break;
        //     case LevelType.Boss:
        //         switch (CurrentLevelIndex)
        //         {
        //             case 4:
        //                 _selectedSystemPrefab = _bossLevelPrefabs[0];
        //                 break;
        //             case 9:
        //                 _selectedSystemPrefab = _bossLevelPrefabs[1];
        //                 break;
        //             case 14:
        //                 _selectedSystemPrefab = _bossLevelPrefabs[2];
        //                 break;
        //         }
        //         break;
        // }

        _selectedSystemPrefab = PickPossibleLevel(_availableNormalLevelPrefabs);
        
        _currentLevel = Instantiate(_selectedSystemPrefab, Vector3.zero, Quaternion.identity);

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

    

    public void OnMMEvent(LevelRandomizeCompleteEvent e)
    {
        if(_currentLevelCount == 5) return;
        if (e.State == EventStateType.OnEventEnd)
        {
            if(_currentLevelCount != 0)
            {
                e.Level.ShiftLevel();
            }
            e.Level.CalculateExitTypes();
            _currentLevelCount++;
            LevelSystem system;
            if (IsNextLevelBossRoom())
            {
                Gate gate = Instantiate(_gatePrefab, e.Level.ExitPosList[0].transform.position, e.Level.ExitPosList[0].transform.rotation).GetComponentInChildren<Gate>();
                gate.SetLevelName(SceneManager.GetActiveScene().name, LevelType.Boss);
                gate.AssignExit(e.Level.ExitPosList[0]); 
                // Instantiating boss level
                switch(_currentBiome)
                {
                    case BiomeType.City:
                        _selectedSystemPrefab = _bossLevelPrefabs[0];
                        break;
                    case BiomeType.Market:
                        _selectedSystemPrefab = _bossLevelPrefabs[1];
                        break;
                    case BiomeType.Shrine:
                        _selectedSystemPrefab = _bossLevelPrefabs[2];
                        break;
                }

                system = Instantiate(_selectedSystemPrefab, e.Level.ExitPosList[0].GetTeleportExit(), Quaternion.identity);
            }
            else
            {
                for (int i = 0; i < e.Level.ExitPosList.Count; i++)
                {
                    Gate gate = Instantiate(_gatePrefab, e.Level.ExitPosList[i].transform.position, e.Level.ExitPosList[i].transform.rotation).GetComponentInChildren<Gate>();
                    gate.SetLevelName(SceneManager.GetActiveScene().name, e.Level.ExitsLevelType[i]);
                    gate.AssignExit(e.Level.ExitPosList[i]);
                    // Instantiating next level

                    switch(e.Level.ExitsLevelType[i])
                    {
                        case LevelType.Reguler:
                            _selectedSystemPrefab = PickPossibleLevel(_availableNormalLevelPrefabs);
                            break;
                        case LevelType.Recover:
                            _selectedSystemPrefab = PickPossibleLevel(_availableRecoveryLevelPrefabs);
                            break;
                        case LevelType.Shop:
                            _selectedSystemPrefab = PickPossibleLevel(_availableShopLevelPrefabs);
                            break;
                    }

                    system = Instantiate(_selectedSystemPrefab, e.Level.ExitPosList[i].GetTeleportExit(), Quaternion.identity);
                }
            }
        }
 
    }

    public void OnMMEvent(EnterNewLevelEvent e)
    {
        CurrentLevelType = e.LevelType;
    }

    public void OnMMEvent(RoomClearedEvent e)
    {
        _doOnce = true;
        //TODO: Spawn exits

        
    }

    public void OnMMEvent(SpawnRewardEvent e)
    {
        switch (CurrentLevelType)
        {
            case LevelType.Reguler:
                _pickupFactory.SpawnPickup(_currentLevel.PickupSpawnPosition.position);
                break;
        }
    }

    private bool IsNextLevelBossRoom()
    {
        return _currentLevelCount == 4;
    }

    public Vector3 GetCurrentPickupSpawnPos(Vector3 offset)
    {
        return new Vector3(_currentLevel.PickupSpawnPosition.position.x + offset.x, _currentLevel.PickupSpawnPosition.position.y + offset.y, _currentLevel.PickupSpawnPosition.position.z + offset.z);
    }
}
