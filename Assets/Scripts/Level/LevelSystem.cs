using System.Collections.Generic;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityUtils;

[RequireComponent(typeof(NavMeshSurface))]
public class LevelSystem : MonoBehaviour
{
    [field: SerializeField, FoldoutGroup("References")] private EnvironmentalObjectSpawner[] _environmentalObjectSpawners;
    [field: SerializeField, FoldoutGroup("References")] private GameplayObjectSpawner[] _gameplayObjectSpawners;
    [SerializeField, FoldoutGroup("References")] private NavMeshSurface _navMeshSurface;
    [SerializeField, FoldoutGroup("References")] private LevelMesh _levelMesh;

    [SerializeField, BoxGroup("Settings")] public BiomeType BiomeType;
    [SerializeField, BoxGroup("Settings"), Range(1, 3)] private int _exitsAmount = 1;
    [SerializeField, BoxGroup("Settings")] private LevelRewardType _possibleRewardTypes;
    [SerializeField, BoxGroup("Settings")] private LevelType _levelType;
    [SerializeField, BoxGroup("Settings")] public ExitsProbability ExitsProbability;
    [SerializeField, BoxGroup("Settings")] public Transform PickupSpawnPosition;
    [field: SerializeField, BoxGroup("Settings")] public SpawnPos[] SpawnPositions { get; private set; }
    [field: SerializeField, BoxGroup("Settings")] public ExitPos[] ExitPositions { get; private set; }

    [SerializeField, BoxGroup("Debug")] public SpawnPos SpawnPos;
    [SerializeField, BoxGroup("Debug")] public PlayerController _player;
    [SerializeField, BoxGroup("Debug")] public List<ExitPos> ExitPosList = new List<ExitPos>();

#if UNITY_EDITOR
    [DisplayAsString(Alignment = TextAlignment.Center, EnableRichText = true, FontSize = 50, Overflow = false), ShowInInspector, HideLabel, BoxGroup()] public string Condition => _environmentalObjectSpawners.IsNullOrEmpty() || SpawnPositions.IsNullOrEmpty() || ExitPositions.IsNullOrEmpty() || _navMeshSurface == null || _levelMesh == null || _gameplayObjectSpawners.IsNullOrEmpty() || _exitsAmount > ExitPositions.Length || PickupSpawnPosition == null ? "STATUS: <color=red>Invalid</color>" : "STATUS: <color=green>Clear</color>";
    [DisplayAsString(Alignment = TextAlignment.Center, EnableRichText = true, FontSize = 20, Overflow = false), ShowInInspector, HideLabel, BoxGroup()] public string Suggestion => _environmentalObjectSpawners.IsNullOrEmpty() || SpawnPositions.IsNullOrEmpty() || ExitPositions.IsNullOrEmpty() || _navMeshSurface == null || _levelMesh == null || _gameplayObjectSpawners.IsNullOrEmpty() || _exitsAmount > ExitPositions.Length || PickupSpawnPosition == null ? "Check references and settings and hit apply setting" : "You are good to go";

    [Button(ButtonHeight = 60)]
    private void ApplySetting()
    {
        _environmentalObjectSpawners = GetComponentsInChildren<EnvironmentalObjectSpawner>();
        _gameplayObjectSpawners = GetComponentsInChildren<GameplayObjectSpawner>();
        SpawnPositions = GetComponentsInChildren<SpawnPos>();
        ExitPositions = GetComponentsInChildren<ExitPos>();
        _navMeshSurface = GetComponent<NavMeshSurface>();
        _levelMesh = GetComponentInChildren<LevelMesh>();
    }
#endif

    void OnValidate()
    {
        if (_navMeshSurface == null) _navMeshSurface = GetComponent<NavMeshSurface>();
        if (_levelMesh == null) _levelMesh = GetComponentInChildren<LevelMesh>();
    }


    void Awake()
    {
        // Alter Level rotation
        float y = Random.Range(-180, 180);
        transform.Rotate(new Vector3(0, y, 0));

        // Generate Environmental Props
        GenerateEnvironmentalProps();

        // Generate Gameplay Props
        GenerateGameplayProps();

        // Rebuild Navmesh
        RebuildNavmesh();

        // Select Spawn/Exit Locations
        SelectSpawnExitLocations();

        Tween.Delay(.5f).OnComplete(() => {
            
            LevelRandomizeCompleteEvent.Trigger(EventStateType.OnEventEnd, SpawnPos.transform);
            LevelRandomizeCompleteEvent.Trigger(EventStateType.OnEventStart, SpawnPos.transform);
        });
    }

    private void SelectSpawnExitLocations()
    {
        if (SpawnPositions.IsNullOrEmpty())
        {
            return;
        }
        int spawnPosIndex = Random.Range(0, SpawnPositions.Length);
        SpawnPos = SpawnPositions[spawnPosIndex];

        if (ExitPositions.IsNullOrEmpty() || _exitsAmount > ExitPositions.Length)
        {
            return;
        }

        if (_exitsAmount == 1)
        {
            int exitPosIndex = Random.Range(0, ExitPositions.Length);
            ExitPosList.Add(ExitPositions[exitPosIndex]);
        }
        else if (_exitsAmount > 1)
        {
            if (_exitsAmount == ExitPositions.Length)
            {
                for (int i = 0; i < ExitPositions.Length; i++)
                {
                    ExitPosList.Add(ExitPositions[i]);
                }
            }
            else
            {
                List<int> indexes = GenerateRandomIndexes(_exitsAmount, ExitPositions.Length, 0);
                for (int i = 0; i < indexes.Count; i++)
                {
                    ExitPosList.Add(ExitPositions[i]);
                }
            }
        }

        
    }

    private void GenerateEnvironmentalProps()
    {
        if (_environmentalObjectSpawners.IsNullOrEmpty())
        {
            return;
        }
        else
        {
            for (int i = 0; i < _environmentalObjectSpawners.Length; i++)
            {
                _environmentalObjectSpawners[i].Respawn();
            }
        }
    }

    private void GenerateGameplayProps()
    {
        if (_gameplayObjectSpawners.IsNullOrEmpty())
        {
            return;
        }
        else
        {
            for (int i = 0; i < _gameplayObjectSpawners.Length; i++)
            {
                _gameplayObjectSpawners[i].Respawn();
            }
        }
    }

    private void RebuildNavmesh()
    {
        _navMeshSurface.BuildNavMesh();
        // Generate Enemies
        
    }

    private List<int> GenerateRandomIndexes(int amount, int maxRange, int minRange = 0)
    {
        List<int> ints = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            if (i == 0)
            {
                ints.Add(Random.Range(minRange, maxRange));
            }
            else
            {
                int newInt = Random.Range(minRange, maxRange);
                while (newInt != ints[i - 1])
                {
                    newInt = Random.Range(minRange, maxRange);
                }
                ints.Add(newInt);
            }

        }

        return ints;

    }
}
    
