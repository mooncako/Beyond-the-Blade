
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
    public int SpawnedThisWave;
    public float NextEligibleTime;
}
