
using System;
using Sirenix.OdinInspector;

[Serializable]
public class EnemyProfile
{
    public string EnemyName;
    public float Difficulty;
    public int Cost;
    public float Weight;
    public int MaxPerWave;
    public float CoolDown;
    [ReadOnly] public int SpawnedThisWave;
    [ReadOnly] public float NextEligibleTime;
}
