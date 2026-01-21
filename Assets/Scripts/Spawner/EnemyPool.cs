using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyPool : MonoBehaviour
{
    [Tooltip("Key: Difficulty Index, Value: Enemy Names")]
    public SerializedDictionary<int, List<string>> EnemyPoolDict = new SerializedDictionary<int, List<string>>();
}
