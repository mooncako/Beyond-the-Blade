using System.Collections.Generic;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityUtils;


public class LevelSystem : MonoBehaviour
{
    [field: SerializeField, FoldoutGroup("References")] private EnvironmentalObjectSpawner[] _environmentalObjectSpawners;
    [field: SerializeField, FoldoutGroup("References")] private GameplayObjectSpawner[] _gameplayObjectSpawners;
    [SerializeField, FoldoutGroup("References")] private LevelMesh _levelMesh;
    [SerializeField, FoldoutGroup("References")] private LevelAssigner _levelAssigner;

    [SerializeField, BoxGroup("Settings")] public BiomeType BiomeType;
    [SerializeField, BoxGroup("Settings")] private LevelRewardType _possibleRewardTypes;
    [SerializeField, BoxGroup("Settings")] private LevelType _levelType;
    public LevelType LevelType => _levelType;
    [SerializeField, BoxGroup("Settings")] public Transform PickupSpawnPosition;
    [field: SerializeField, BoxGroup("Settings")] public SpawnPos[] SpawnPositions { get; private set; }
    [field: SerializeField, BoxGroup("Settings")] public EnemySpawnPos[] EnemySpawnPositions { get; private set; }
    [field: SerializeField, BoxGroup("Settings")] public ExitPos[] ExitPositions { get; private set; }

    [SerializeField, BoxGroup("Debug")] public SpawnPos SpawnPos;
    [SerializeField, BoxGroup("Debug")] public PlayerController _player;
    [SerializeField, BoxGroup("Debug")] public List<ExitPos> ExitPosList = new List<ExitPos>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] public List<LevelType> ExitsLevelType = new List<LevelType>();

#if UNITY_EDITOR
    [DisplayAsString(Alignment = TextAlignment.Center, EnableRichText = true, FontSize = 50, Overflow = false), ShowInInspector, HideLabel, BoxGroup()] public string Condition => _environmentalObjectSpawners.IsNullOrEmpty() || SpawnPositions.IsNullOrEmpty() || ExitPositions.IsNullOrEmpty()  || _levelMesh == null || _gameplayObjectSpawners.IsNullOrEmpty() || PickupSpawnPosition == null || _levelAssigner == null ? "STATUS: <color=red>Invalid</color>" : "STATUS: <color=green>Clear</color>";
    [DisplayAsString(Alignment = TextAlignment.Center, EnableRichText = true, FontSize = 20, Overflow = false), ShowInInspector, HideLabel, BoxGroup()] public string Suggestion => _environmentalObjectSpawners.IsNullOrEmpty() || SpawnPositions.IsNullOrEmpty() || ExitPositions.IsNullOrEmpty() || _levelMesh == null || _gameplayObjectSpawners.IsNullOrEmpty() || PickupSpawnPosition == null || _levelAssigner == null ? "Check references and settings and hit apply setting" : "You are good to go";

    [Button(ButtonHeight = 60)]
    private void ApplySetting()
    {
        _environmentalObjectSpawners = GetComponentsInChildren<EnvironmentalObjectSpawner>();
        _gameplayObjectSpawners = GetComponentsInChildren<GameplayObjectSpawner>();
        SpawnPositions = GetComponentsInChildren<SpawnPos>();
        EnemySpawnPositions = GetComponentsInChildren<EnemySpawnPos>();
        ExitPositions = GetComponentsInChildren<ExitPos>();
        _levelMesh = GetComponentInChildren<LevelMesh>();
        _levelAssigner = GetComponentInChildren<LevelAssigner>();
    }
#endif

    void OnValidate()
    {
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
        // RebuildNavmesh();

        // Select Spawn Position
        if (SpawnPositions.IsNullOrEmpty())
        {
            return;
        }
        int spawnPosIndex = Random.Range(0, SpawnPositions.Length);
        SpawnPos = SpawnPositions[spawnPosIndex];

        Tween.Delay(.01f).OnComplete(() => {
            
            LevelRandomizeCompleteEvent.Trigger(EventStateType.OnEventEnd, SpawnPos.transform, this);

            Tween.Delay(.01f).OnComplete(() =>
            {
                LevelRandomizeCompleteEvent.Trigger(EventStateType.OnEventStart, SpawnPos.transform, this);
            });
            
        });
    }

    public void CalculateExitTypes(ExitsProbability exitsProbability)
    {
        float possibilityIndex;
        ExitsLevelType.Clear();
        for (int i = 0; i < ExitPosList.Count; i++)
        {
            possibilityIndex = Random.Range(0f, 1f);
            if (possibilityIndex <= exitsProbability.RegularExitPercentage)
            {
                ExitsLevelType.Add(LevelType.Regular);
            }
            else if (possibilityIndex <= exitsProbability.RegularExitPercentage + exitsProbability.RecoveryExitPercentage)
            {
                ExitsLevelType.Add(LevelType.Recover);
            }
            else
            {
                ExitsLevelType.Add(LevelType.Shop);
            }
        }
    }

    public void AssignExitTypes(List<LevelType> exitTypes)
    {
        for(int i = 0; i < exitTypes.Count; i++)
        {
            ExitsLevelType.Add(exitTypes[i]);
        }
    }

    public void SelectExitLocations(int exitsAmount)
    {

        if (ExitPositions.IsNullOrEmpty() || exitsAmount > ExitPositions.Length)
        {
            if(exitsAmount > ExitPositions.Length)
                Debug.LogError($"{gameObject} Exit amount exceeded, {exitsAmount} exit positions. Available exit positions: {ExitPositions.Length}");
            else
                Debug.LogError($"{gameObject} No exit positions found");
            return;
        }

        if (exitsAmount == 1)
        {
            int exitPosIndex = Random.Range(0, ExitPositions.Length);
            ExitPosList.Add(ExitPositions[exitPosIndex]);
        }
        else if (exitsAmount > 1)
        {
            if (exitsAmount == ExitPositions.Length)
            {
                for (int i = 0; i < ExitPositions.Length; i++)
                {
                    ExitPosList.Add(ExitPositions[i]);
                }
            }
            else
            {
                List<int> indexes = GenerateRandomIndexes(exitsAmount, ExitPositions.Length, 0);
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

    // private void RebuildNavmesh()
    // {
    //     _navMeshSurface.BuildNavMesh();
    //     // Generate Enemies
        
    // }

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

    public void ShiftLevel()
    {
        Vector3 shiftDistance = transform.position - SpawnPos.transform.position;
        transform.position += shiftDistance;
    }
}
    
