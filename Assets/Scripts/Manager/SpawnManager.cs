using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MMSingleton<SpawnManager>, MMEventListener<EnemyClearedEvent>, MMEventListener<LevelRandomizeCompleteEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("References")] private EnemyDatabaseSO _enemyDatabase;
    [SerializeField, BoxGroup("Settings")] private GameDifficultyDataSO _gameDifficultySettings;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _minDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _maxDifficulty = 0;
    
    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _maxEnemyCountPerWave;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _canSpawn = true;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private List<(string enemyName, float timeOffset)> _picks = new List<(string, float)>();
#if UNITY_EDITOR
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] public List<string> CurrentSpawningEnemies => _currentSpawningEnemies.ToList();
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] public List<string> EnemiesWaitingForSpawn => _enemiesWaitingForSpawn.ToList();
#endif
    [field: SerializeField] private Dictionary<EnemyProfile, GameObject> _currentEnemyDict = new Dictionary<EnemyProfile, GameObject>();

    private int _budget;

    private Queue<string> _currentSpawningEnemies = new Queue<string>();
    private Queue<string> _enemiesWaitingForSpawn = new Queue<string>();

    private void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    protected override void Awake()
    {
        base.Awake();

        UpdateEnemyList();
        DontDestroyOnLoad(this);

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.MMEventStartListening<EnemyClearedEvent>();
        this.MMEventStartListening<LevelRandomizeCompleteEvent>();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<EnemyClearedEvent>();
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<EnemyClearedEvent>();
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
    }

    

    public void OnMMEvent(EnemyClearedEvent e)
    {
        SpawnWave();
    }

    public void OnMMEvent(LevelRandomizeCompleteEvent e)
    {
        if (e.State == EventStateType.OnEventStart)
        {

            if (_canSpawn)
            {
                SetupWaveInfo();
            }
        }
        
    }

    [Button]
    private void UpdateEnemyList()
    {
        _currentEnemyDict.Clear();
        _picks.Clear();
        _minDifficulty = _gameDifficultySettings.MinDifficultyCurve.Evaluate(LevelManager.Instance.CurrentLevelIndex / _gameDifficultySettings.TotalLevelCount);
        _maxDifficulty = _gameDifficultySettings.MaxDifficultyCurve.Evaluate(LevelManager.Instance.CurrentLevelIndex / _gameDifficultySettings.TotalLevelCount);
        List<GameObject> poolList = new List<GameObject>();

        foreach (EnemyProfile profile in _enemyDatabase.EnemyDict.Keys)
        {
            if (profile.Difficulty >= _minDifficulty && profile.Difficulty <= _maxDifficulty)
            {
                _currentEnemyDict.Add(profile, _enemyDatabase.EnemyDict[profile]);
                poolList.Add(_enemyDatabase.EnemyDict[profile]);
            }
        }

        _pool.InitializeRuntimePool(poolList);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "MainMenu")
        {
            ResetManager();
        }
    }

    private void SetupWaveInfo()
    {
        List<EnemyProfile> enemies = new List<EnemyProfile>();
        foreach (var profile in _currentEnemyDict.Keys)
        {
            profile.SpawnedThisWave = 0;
            profile.NextEligibleTime = 0;
            enemies.Add(profile);
        }
        _budget = _gameDifficultySettings.StartingWaveBudget * Mathf.RoundToInt(Mathf.Pow(_gameDifficultySettings.BudgetScale, LevelManager.Instance.CurrentLevelIndex-1));
        _maxEnemyCountPerWave = Mathf.RoundToInt(_gameDifficultySettings.StartingEnemyCountPerWave * _gameDifficultySettings.MaxWaveEnemyCountMultiplierCurve.Evaluate(LevelManager.Instance.CurrentLevelIndex));
        float timer = _gameDifficultySettings.StartingSpawnTimer * _gameDifficultySettings.SpawnTimerMultiplierCurve.Evaluate(LevelManager.Instance.CurrentLevelIndex);
        _picks = WaveSpawner.GenerateWaveScheduled(enemies, _budget, .25f);

        for (int i = 0; i < _picks.Count; i++)
        {
            _enemiesWaitingForSpawn.Enqueue(_picks[i].enemyName);
        }

        EncounterStartEvent.Trigger(timer);
        SpawnWave();
    }

    [Button]
    private void SpawnWave()
    {
        if (_enemiesWaitingForSpawn.Count >= _maxEnemyCountPerWave)
        {
            for (int i = 0; i < _maxEnemyCountPerWave; i++)
            {
                _currentSpawningEnemies.Enqueue(_enemiesWaitingForSpawn.Dequeue());
            }
        }
        else if (_enemiesWaitingForSpawn.Count > 0)
        {
            while (_enemiesWaitingForSpawn.Count > 0)
            {
                _currentSpawningEnemies.Enqueue(_enemiesWaitingForSpawn.Dequeue());
            }
        }
        else
        {
            return;
        }

        while (_currentSpawningEnemies.Count > 0)
        {
            GameObject enemy = _enemyDatabase.GetEnemy(_currentSpawningEnemies.Dequeue());
            SpawnEnemy(enemy);
            EnemySpawnedEvent.Trigger(enemy.GetComponent<Health>());
        }
    }

    [Button]
    private void SpawnEnemy(GameObject prefab)
    {
        CustomCharacterMovement movement = _pool.Get(prefab).GetComponent<CustomCharacterMovement>();
        movement.Teleport(AIUtil.GetRandomPointOnNavMesh());
    }
    
    private void ResetManager()
    {
        _minDifficulty = 0;
    }

    
}
