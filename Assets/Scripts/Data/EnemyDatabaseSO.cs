using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "BytheBlade/EnemyDatabase")]
public class EnemyDatabaseSO : SerializedScriptableObject
{
    public Dictionary<EnemyProfile, GameObject> EnemyDict = new Dictionary<EnemyProfile, GameObject>();
}
