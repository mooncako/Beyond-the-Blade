using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackSensorConfig", menuName = "AI/AttackSensorConfig")]
public class AttackSensorConfigSO : ScriptableObject
{
    public float CombatSensorRadius = 5;
    public float CloseRangeSensorRadius = 1;
    public float AttackDelay = 1;
    public LayerMask AttackableLayerMask;
}


