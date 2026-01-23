
using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class EnemyProfile
{
    [Header("Enemy Name")]
    public string EnemyName;
    [Header("Biome")]
    public BiomeType BiomeType;
    [Header("Difficulty Index")]
    public float DifficultyIndex;
    [Header("MaxPerWave")]
    public int MaxPerWave;
    [Header("Cooldown")]
    public float Cooldown;
    [Header("SpawnedThisWave")]
    [ReadOnly] public int SpawnedThisWave;
    [Header("NextEligibleTime")]
    [ReadOnly] public float NextEligibleTime;
}
