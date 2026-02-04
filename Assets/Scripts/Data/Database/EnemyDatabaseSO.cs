using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "BytheBlade/EnemyDatabase")]
public class EnemyDatabaseSO : SerializedScriptableObject
{
    public Dictionary<EnemyProfile, GameObject> EnemyDict = new Dictionary<EnemyProfile, GameObject>();

    public GameObject GetEnemy(string enemyName, EnemySpawnerType spawnerType)
    {
        for (int i = 0; i < EnemyDict.Count; i++)
        {
            if (EnemyDict.Keys.ElementAt(i).EnemyName == enemyName)
            {
                return EnemyDict.ElementAt(i).Value;
            }
        }

        return null;
    }
}
