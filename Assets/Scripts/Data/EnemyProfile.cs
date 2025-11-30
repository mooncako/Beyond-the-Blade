
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
    [Header("Difficulty")]
    public float Difficulty;
    [Header("Cost")]
    public int Cost;
    [Header("Weight")]
    public float Weight;
    [Header("MaxPerWave")]
    public int MaxPerWave;
    [Header("Cooldown")]
    public float Cooldown;
    [Header("SpawnedThisWave")]
    [ReadOnly] public int SpawnedThisWave;
    [Header("NextEligibleTime")]
    [ReadOnly] public float NextEligibleTime;
}
