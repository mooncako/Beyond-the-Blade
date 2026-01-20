using System.Collections.Generic;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityUtils;


public class LevelSystem : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")] private LevelMesh _levelMesh;
    [SerializeField, FoldoutGroup("References")] private LevelAssigner _levelAssigner;

    [SerializeField, BoxGroup("Settings")] private LevelRewardType _possibleRewardTypes;

    [SerializeField, BoxGroup("Settings")] public Transform PickupSpawnPosition;


    void OnValidate()
    {
        if (_levelMesh == null) _levelMesh = GetComponentInChildren<LevelMesh>();
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
    
