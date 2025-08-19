using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("References")] private EnemyDatabaseSO _enemyDatabase;
    [SerializeField, BoxGroup("Settings")] private GameDifficultyDataSO _gameDifficultySettings;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _minDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _maxDifficulty = 0;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentLevelIndex = 0;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] private List<GameObject> _currentEnemyList = new List<GameObject>();

    private void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += ResetManager;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetManager;
    }

    

    protected override void Awake()
    {
        base.Awake();
        
        UpdateEnemyList();
    }

    private void UpdateEnemyList()
    {
        _currentEnemyList.Clear();
        _minDifficulty = _gameDifficultySettings.MinDifficultyCurve.Evaluate(_currentLevelIndex / _gameDifficultySettings.TotalLevelCount);
        _maxDifficulty = _gameDifficultySettings.MaxDifficultyCurve.Evaluate(_currentLevelIndex / _gameDifficultySettings.TotalLevelCount);

        foreach (EnemyProfile profile in _enemyDatabase.EnemyDict.Keys)
        {
            if (profile.Difficulty >= _minDifficulty && profile.Difficulty <= _maxDifficulty)
            {
                _currentEnemyList.Add(_enemyDatabase.EnemyDict[profile]);
            }
        }

        _pool.InitializeRuntimePool(_currentEnemyList);
    }

    private void ResetManager(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "MainMenu")
        {
            _minDifficulty = 0;
            _currentLevelIndex = 0;
        }
    }

}
