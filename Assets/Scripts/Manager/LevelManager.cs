using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("References")] private EnemyDatabaseSO _enemyDatabase;
    [SerializeField, BoxGroup("Settings")] private GameDifficultyDataSO _gameDifficultySettings;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _minDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _maxDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentLevelIndex = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _canSpawn = true;
    [field: SerializeField, BoxGroup("Debug")] private Dictionary<EnemyProfile, GameObject> _currentEnemyDict = new Dictionary<EnemyProfile, GameObject>();

    private int _budget;

    private void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    protected override void Awake()
    {
        base.Awake();

        UpdateEnemyList();

        

        DontDestroyOnLoad(this);

    }

    void Start()
    {
        if (_canSpawn)
        {
            SpawnWave();
        }
    }

    [Button]
    private void UpdateEnemyList()
    {
        _currentEnemyDict.Clear();
        _minDifficulty = _gameDifficultySettings.MinDifficultyCurve.Evaluate(_currentLevelIndex / _gameDifficultySettings.TotalLevelCount);
        _maxDifficulty = _gameDifficultySettings.MaxDifficultyCurve.Evaluate(_currentLevelIndex / _gameDifficultySettings.TotalLevelCount);
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

    [Button]
    private void SpawnWave()
    {
        List<EnemyProfile> enemies = new List<EnemyProfile>();
        foreach (var profile in _currentEnemyDict.Keys)
        {
            profile.SpawnedThisWave = 0;
            profile.NextEligibleTime = 0;
            enemies.Add(profile);
        }
        _budget = _gameDifficultySettings.StartingWaveBudget * Mathf.RoundToInt(Mathf.Pow(_gameDifficultySettings.BudgetScale, _currentLevelIndex));
        var picks = WaveSpawner.GenerateWaveScheduled(enemies, _budget, .25f);
        Debug.Log(picks.Count);
        foreach (var profile in picks)
        {
            Debug.Log(profile.EnemyName);
        }
    }

    

    private void ResetManager()
    {
        _minDifficulty = 0;
        _currentLevelIndex = 0;
    }

    

    [Button]
    private void SpawnEnemy(GameObject prefab)
    {
        CustomCharacterMovement movement = _pool.Get(prefab).GetComponent<CustomCharacterMovement>();
        movement.Teleport(AIUtil.GetRandomPointOnNavMesh());
    }

}
