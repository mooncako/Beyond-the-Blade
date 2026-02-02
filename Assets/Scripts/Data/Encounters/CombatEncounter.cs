using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[Serializable]
public class CombatEncounter: BaseEncounter
{
    public EnemySpawnModeType SpawnModeType; 
    [SerializeField, BoxGroup("Debug"), ReadOnly] public int EnemyCount;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<Health> _enemies;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _startCountingEnemies = false;

    public CombatEncounter(int enemyCount, string encounterID, EnemySpawnModeType spawnModeType)
    {
        EnemyCount = enemyCount;
        EncounterID = encounterID;
        SpawnModeType = spawnModeType;
        _enemies = new List<Health>();
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void AddEnemy(Health enemy)
    {
        if(!_enemies.Contains(enemy))
        {
            if(!_startCountingEnemies)
                _startCountingEnemies = true;
            _enemies.Add(enemy);
        }
            
    }

    public void RemoveEnemy(Health enemy)
    {
        if(_enemies.Contains(enemy))
            _enemies.Remove(enemy);
    }

    protected override bool CheckCompletion()
    {
        if(IsStarted && _startCountingEnemies)
        {
            return _enemies.Count == 0;
        }
        else
        {
            return false;
        }
    }

    public override void Copy(BaseEncounter other)
    {
        base.Copy(other);
        if(other is CombatEncounter ce)
        {
            SpawnModeType = ce.SpawnModeType;
        }
    }

}
