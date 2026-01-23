using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MMSingleton<LevelManager>,
    MMEventListener<RoomClearedEvent>,
    MMEventListener<SpawnRewardEvent>,
    MMEventListener<EnterNewLevelEvent>
{

    [SerializeField, BoxGroup("References")] private PickupFactory _pickupFactory;

    [SerializeField, BoxGroup("Debug")] public BiomeType CurrentBiome;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public LevelType CurrentLevelType;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LevelSystem _currentLevel;
    [SerializeField, HideInInspector] public LevelSystem CurrentLevel => _currentLevel;

    [SerializeField, BoxGroup("Debug"), ReadOnly] public float CurrentLevelIndex = 0;


    void OnValidate()
    {
        if (_pickupFactory == null) _pickupFactory = GetComponent<PickupFactory>();
    }

    protected override void Awake()
    {
        base.Awake();


    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.MMEventStartListening<RoomClearedEvent>();
        this.MMEventStartListening<SpawnRewardEvent>();
        this.MMEventStartListening<EnterNewLevelEvent>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<RoomClearedEvent>();
        this.MMEventStopListening<SpawnRewardEvent>();
        this.MMEventStopListening<EnterNewLevelEvent>();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        this.MMEventStopListening<RoomClearedEvent>();
        this.MMEventStopListening<SpawnRewardEvent>();
        this.MMEventStopListening<EnterNewLevelEvent>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == SCENENAME.Menu || scene.name == SCENENAME.Hub)
        {
            ResetManager();
        }

    }

    private void ResetManager()
    {
        CurrentLevelIndex = 0;
    }

    public void OnMMEvent(EnterNewLevelEvent e)
    {
        CurrentLevelType = e.LevelType;
    }

    public void OnMMEvent(RoomClearedEvent e)
    {

        
    }

    public void OnMMEvent(SpawnRewardEvent e)
    {

        _pickupFactory.SpawnPickup(_currentLevel.PickupSpawnPosition.position);
    }

    public Vector3 GetCurrentPickupSpawnPos(Vector3 offset)
    {
        return new Vector3(_currentLevel.PickupSpawnPosition.position.x + offset.x, _currentLevel.PickupSpawnPosition.position.y + offset.y, _currentLevel.PickupSpawnPosition.position.z + offset.z);
    }

    public void ChangeLevel(LevelSystem system)
    {
        CurrentLevelIndex++;
        _currentLevel = system;
        // EnemyStartSpawnEvent.Trigger(_currentLevel.EnemySpawnPositions);
    }
}
