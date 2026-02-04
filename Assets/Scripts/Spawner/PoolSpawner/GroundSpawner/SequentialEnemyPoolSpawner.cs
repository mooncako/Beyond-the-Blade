using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SequentialEnemyPoolSpawner : PoolSpawner
{
    

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    void OnEnable()
    {
        _spawnTrigger.TriggerEnter.AddListener(OnSpawnTriggerEnter);
    }

    void OnDisable()
    {
        _spawnTrigger.TriggerEnter.RemoveListener(OnSpawnTriggerEnter);
    }

    private void OnSpawnTriggerEnter(Collider other)
    {
        SpawnEnemies();
    }
}
