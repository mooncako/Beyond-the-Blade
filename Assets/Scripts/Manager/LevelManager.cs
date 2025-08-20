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
    [SerializeField, BoxGroup("Settings")] private int _noMorethanNInARow = 2;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _minDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _maxDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentLevelIndex = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _canSpawn = true;
    [SerializeField, ReadOnly] private int _minCost = 0;
    [field: SerializeField, BoxGroup("Debug")] private Dictionary<EnemyProfile, GameObject> _currentEnemyDict = new Dictionary<EnemyProfile, GameObject>();

    private NavMeshTriangulation _triangulation;

    System.Random _random = new System.Random();
    readonly Queue<string> _lastPicks = new Queue<string>();


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

        CalculateMinCost();

        DontDestroyOnLoad(this);

    }

    void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        // if (_canSpawn)
        // {
        //     SpawnWave();
        // }
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
        int budget = Mathf.RoundToInt(_gameDifficultySettings.StartingWaveBudget * Mathf.Pow(_gameDifficultySettings.BudgetScale, _currentLevelIndex));
        _currentLevelIndex++;

        _lastPicks.Clear();

        var now = Time.time;
        var picks = new List<EnemyProfile>();
        var eligible = new List<EnemyProfile>();

        int guard = 1000;

        while (budget >= _minCost && guard-- > 0)
        {
            foreach (EnemyProfile profile in _currentEnemyDict.Keys)
            {
                if (profile.Cost <= budget && profile.SpawnedThisWave < profile.MaxPerWave && now >= profile.NextEligibleTime && !WouldViolateAntiRepeat(profile.EnemyName))
                {
                    eligible.Add(profile);
                }
            }

            if (eligible.Count == 0) break;

            var choice = WeightedPick(eligible);
            picks.Add(choice);
            budget -= choice.Cost;
            foreach (EnemyProfile profile in _currentEnemyDict.Keys)
            {
                if (profile.EnemyName == choice.EnemyName)
                {
                    profile.SpawnedThisWave++;
                    profile.NextEligibleTime = now + profile.CoolDown;
                    RememberPick(profile.EnemyName);
                }
            }
        }

        foreach (var def in picks)
        {
            Debug.Log(1);
            SpawnEnemy(_currentEnemyDict[def]);
        }
    }

    private bool WouldViolateAntiRepeat(string id)
    {
        if (_noMorethanNInARow <= 0) return false;

        if (_lastPicks.Count < _noMorethanNInARow - 1) return false;

        foreach (var name in _lastPicks)
        {
            if (name == id)
            {
                return true;
            }
        }
        return false;
    }

    private void RememberPick(string id)
    {
        if (_noMorethanNInARow <= 0) return;
        _lastPicks.Enqueue(id);
        while (_lastPicks.Count > _noMorethanNInARow - 1)
        {
            _lastPicks.Dequeue();
        }
    }

    private EnemyProfile WeightedPick(List<EnemyProfile> list)
    {
        double total = 0;
        var cumul = new List<(EnemyProfile profile, double cum)>(list.Count);
        foreach (var profile in list)
        {
            var w = Math.Max(1e-6, profile.Weight / Mathf.Max(1, profile.Cost));
            total += w;
            cumul.Add((profile, total));
        }

        var r = _random.NextDouble() * total;
        foreach (var (def, cum) in cumul)
        {
            if (r <= cum) return def;
        }
        return list[list.Count - 1];
    }

    private void ResetManager()
    {
        _minDifficulty = 0;
        _currentLevelIndex = 0;
    }

    private void CalculateMinCost()
    {
        foreach (EnemyProfile profile in _enemyDatabase.EnemyDict.Keys)
        {
            if (_minCost == 0)
            {
                _minCost = profile.Cost;
            }
            else
            {
                if (_minCost > profile.Cost)
                {
                    _minCost = profile.Cost;
                }
            }
        }
    }

    [Button]
    private void SpawnEnemy(GameObject prefab)
    {
        CustomCharacterMovement movement = _pool.Get(prefab).GetComponent<CustomCharacterMovement>();
        Debug.Log(movement.gameObject);
        movement.Teleport(AIUtil.GetRandomSpawnPos(_triangulation));
    }

}
