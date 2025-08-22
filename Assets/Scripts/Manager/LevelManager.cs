using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MMSingleton<LevelManager>
{
    [SerializeField, BoxGroup("References")] private LevelSystem[] _availableNormalLevelPrefabs;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);


    }
}
