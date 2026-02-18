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
        
    }

    void OnDisable()
    {
        
    }

    public void TriggerSpawn()
    {
        SpawnEnemies();
    }
}
