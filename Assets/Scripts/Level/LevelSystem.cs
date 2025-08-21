using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] public BiomeType BiomeType;
    [field: SerializeField, BoxGroup("Settings")] public SpawnPos[] SpawnPositions { get; private set; }
    [field: SerializeField, BoxGroup("Settings")] public ExitPos[] ExitPositions { get; private set; }

    [SerializeField, BoxGroup("Debug")] public SpawnPos SpawnPos;
    [SerializeField, BoxGroup("Debug")] public List<ExitPos> ExitPosList;
    
}
