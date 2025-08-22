using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableShopLevelPrefabs;
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableReceoveryLevelPrefabs;
    [SerializeField, BoxGroup("Debug")] private BiomeType _currentBiome;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);


    }
}
