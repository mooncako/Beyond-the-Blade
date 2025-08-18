using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackSensorConfig", menuName = "AI/AttackSensorConfig")]
public class AttackSensorConfigSO : ScriptableObject
{
    public float SensorRadius = 10;
    public float AttackRadius = 1f;
    public int AttackCost = 1;
    public float AttackDelay = 1;
    public LayerMask AttackableLayerMask;
}


