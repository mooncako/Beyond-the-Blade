using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Stats/EnemyStats")]
public class EnemyStatsSO : Stats
{
    [Header("Enemy Stats")]
    public float StunThreshold = 5;
    public float WalkSpeedMultiplier = .5f;
    public float RunSpeedMultiplier = 1f;
}
