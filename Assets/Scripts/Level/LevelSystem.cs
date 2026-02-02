using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityUtils;


public class LevelSystem : MonoBehaviour,
    MMEventListener<EncounterClearEvent>
{
    [SerializeField, FoldoutGroup("References")] private LevelMesh _levelMesh;
    [SerializeField, FoldoutGroup("References")] private LevelAssigner _levelAssigner;
    [SerializeField, FoldoutGroup("References")] private Transform _playerSpawnPos;
    public Transform PlayerSpawnPos => _playerSpawnPos;

    [SerializeField, BoxGroup("Settings")] private LevelRewardType _possibleRewardType;
    public LevelRewardType PossibleRewardType => _possibleRewardType;

    [SerializeField, BoxGroup("Settings")] public Transform PickupSpawnPosition;
    [SerializeField, BoxGroup("Settings")] private string[] _encounterClearRequirements;


    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<string> _currentEncounters;


    void OnValidate()
    {
        if (_levelMesh == null) _levelMesh = GetComponentInChildren<LevelMesh>();
        if (_levelAssigner == null) _levelAssigner = GetComponentInChildren<LevelAssigner>();
    }

    void Awake()
    {
        for(int i = 0; i < _encounterClearRequirements.Length; i++)
        {
            _currentEncounters.Add(_encounterClearRequirements[i]);
        }
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

    public void OnMMEvent(EncounterClearEvent e)
    {
        if(_currentEncounters.Contains(e.EncounterID))
        {
            _currentEncounters.Remove(e.EncounterID);


            if(_currentEncounters.Count == 0)
            {
                LevelClearedEvent.Trigger(LevelManager.Instance.CurrentLevel.PossibleRewardType, true);
            }
        }
    }
}
    
