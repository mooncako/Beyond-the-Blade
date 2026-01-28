using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
#endif
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

public class SpawnManager : MMSingleton<SpawnManager>,
    MMEventListener<EnemyClearedEvent>,
    MMEventListener<EnemyStartSpawnEvent>,
    MMEventListener<ReturnEnemyEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("References")] private EnemyDatabaseSO _enemyDatabase;
    [SerializeField, BoxGroup("Settings")] private GameDifficultyDataSO _gameDifficultySettings;
    [SerializeField, BoxGroup("Settings")] private float _minSpawnDelay = .1f;
    [SerializeField, BoxGroup("Settings")] private float _maxSpawnDelay = .4f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _difficultyIndex = 1;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _encounterAmount = 0;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _maxEnemyCountPerWave;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _canSpawn = true;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private List<(string enemyName, float timeOffset)> _picks = new List<(string, float)>();

    [field: SerializeField] private Dictionary<EnemyProfile, GameObject> _currentEnemyDict = new Dictionary<EnemyProfile, GameObject>();

#if UNITY_EDITOR
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] public List<string> CurrentSpawningEnemies => _currentSpawningEnemies.ToList();
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] public List<string> EnemiesWaitingForSpawn => _enemiesWaitingForSpawn.ToList();
#endif
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _poolInitialized = false;

    private Queue<string> _currentSpawningEnemies = new Queue<string>();
    private Queue<string> _enemiesWaitingForSpawn = new Queue<string>();

    [SerializeField, ReadOnly] private EnemySpawnPos[] _enemySpawnPos;

    private void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        UpdateEnemyList();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.MMEventStartListening<EnemyClearedEvent>();
        this.MMEventStartListening<EnemyStartSpawnEvent>();
        this.MMEventStartListening<ReturnEnemyEvent>();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<EnemyClearedEvent>();
        this.MMEventStopListening<EnemyStartSpawnEvent>();
        this.MMEventStopListening<ReturnEnemyEvent>();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<EnemyClearedEvent>();
        this.MMEventStopListening<EnemyStartSpawnEvent>();
        this.MMEventStopListening<ReturnEnemyEvent>();
    }



    public void OnMMEvent(EnemyClearedEvent e)
    {
        // SpawnWave();
    }

    public void OnMMEvent(EnemyStartSpawnEvent e)
    { 
        if (_canSpawn)
        {
            Array.Clear(_enemySpawnPos, 0, _enemySpawnPos.Length);
            _enemySpawnPos = e.EnemySpawnPositions;
            _maxEnemyCountPerWave = UnityEngine.Random.Range(e.MinEnemyCountPerWave, e.MaxEnemyCountPerWave + 1);
            SetupSpawnInfo(e.EnemySpawnerType, e.SpawnPositionType, e.EnemyPool, e.IsPrecisePos);
            
        }
        
    }

    public void OnMMEvent(ReturnEnemyEvent e)
    {
        _pool.Return(e.Go.transform.parent.gameObject);
    }

    [Button]
    public void UpdateEnemyList()
    {
        if(_poolInitialized) return;
        //TODO: rewrite this
        _poolInitialized = true;
        _currentEnemyDict.Clear();
        
        List<GameObject> poolList = new List<GameObject>();

        foreach (EnemyProfile profile in _enemyDatabase.EnemyDict.Keys)
        {
            if (profile.BiomeType == LevelManager.Instance.CurrentBiome)
            {
                _currentEnemyDict.Add(profile, _enemyDatabase.EnemyDict[profile]);
                poolList.Add(_enemyDatabase.EnemyDict[profile]);
            }
        }

        _pool.InitializeRuntimePool(poolList);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == SCENENAME.Hub)
        {
            ResetManager();
        }
        
    }

    private void SetupSpawnInfo(EnemySpawnerType spawnerType, EnemySpawnModeType spawnPositionType, EnemyPool enemyPool, bool isPrecisePos)
    {
        List<EnemyProfile> enemies = new List<EnemyProfile>();
        foreach (var profile in _currentEnemyDict.Keys)
        {
            profile.SpawnedThisWave = 0;
            profile.NextEligibleTime = 0;
            if(enemyPool.Enemies.Contains(profile.EnemyName) && profile.EnemySpawnerType == spawnerType)
                enemies.Add(profile);
        }

        switch(spawnerType)
        {
            case EnemySpawnerType.Train:
                PickEnemiesToSpawn(enemies, 0f);
                break;
            case EnemySpawnerType.Ground:
                PickEnemiesToSpawn(enemies, 0f);
                break;
        }
        
        switch(spawnPositionType)
        {
            case EnemySpawnModeType.Wave:
                StartWave(isPrecisePos);
                break;
            case EnemySpawnModeType.Fixed:
                StartFixed(isPrecisePos);
                break;
        }
    }

    private void PickEnemiesToSpawn(List<EnemyProfile> enemies, float timeOffset)
    {
        _picks.Clear();
        int enemyCount = 0;
        while (enemyCount < _maxEnemyCountPerWave)
        {
            List<EnemyProfile> eligibleEnemies = new List<EnemyProfile>();
            foreach (EnemyProfile ep in enemies)
            {
                if (ep.NextEligibleTime <= Time.time)
                {
                    eligibleEnemies.Add(ep);
                }
            }

            if (eligibleEnemies.Count == 0) break;



            EnemyProfile pickedEnemy = AIUtil.PickEnemyBasedOnDifficultyIndex(eligibleEnemies, _difficultyIndex);
            if (pickedEnemy != null)
            {
                _picks.Add((pickedEnemy.EnemyName, timeOffset));
                pickedEnemy.SpawnedThisWave++;
                pickedEnemy.NextEligibleTime = Time.time + pickedEnemy.Cooldown;
                enemyCount++;
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < _picks.Count; i++)
        {
            _enemiesWaitingForSpawn.Enqueue(_picks[i].enemyName);
        }
    }

    [Button]
    private void StartWave(bool isPrecisePos)
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
            EnemySpawnStoppedEvent.Trigger();
            return;
        }

        StopCoroutine(SpawnEnemyCO(isPrecisePos));
        StartCoroutine(SpawnEnemyCO(isPrecisePos));
    }

    private void StartFixed(bool isPrecisePos)
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
            EnemySpawnStoppedEvent.Trigger();
            return;
        }

        int currentSpawningEnemyCount = _currentSpawningEnemies.Count;
        _enemySpawnPos.Shuffle();

        for(int i = 0; i < currentSpawningEnemyCount; i++)
        {
            SpawnEnemyFixed(_enemyDatabase.GetEnemy(_currentSpawningEnemies.Dequeue()), _enemySpawnPos[i], isPrecisePos);
        }
    }
    
    private IEnumerator SpawnEnemyCO(bool isPrecisePos)
    {
        while (_currentSpawningEnemies.Count > 0)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay));
            GameObject enemy = _enemyDatabase.GetEnemy(_currentSpawningEnemies.Dequeue());
            SpawnEnemyWave(enemy, isPrecisePos);
        }
        
    }

    [Button]
    private void SpawnEnemyWave(GameObject prefab, bool isPrecisePos)
    {
        EnemySpawner spawner = _pool.Get(prefab).GetComponent<EnemySpawner>();
        if(!isPrecisePos)
            spawner.transform.position = AIUtil.GetRandomSpawnPosFromArry(_enemySpawnPos);
        else
            spawner.transform.position = AIUtil.GetRandomSpawnPosFromArry(_enemySpawnPos, true);
        spawner.StartSpawn();
    }

    private void SpawnEnemyFixed(GameObject prefab, EnemySpawnPos spawnPos, bool isPrecisePos)
    {
        EnemySpawner spawner = _pool.Get(prefab).GetComponent<EnemySpawner>();
        if(!isPrecisePos)
            spawner.transform.position = AIUtil.GetRandomSpawnPos(spawnPos);
        else
            spawner.transform.position = AIUtil.GetRandomSpawnPos(spawnPos, true);
        spawner.StartSpawn();
    }

    private void ResetManager()
    {
        _difficultyIndex = _gameDifficultySettings.StartingDifficultyIndex;
        _encounterAmount = 0;
        _poolInitialized = false;
    }

    public void ToggleSpawn(bool toggle)
    {
        _canSpawn = toggle;
    }

    
}
