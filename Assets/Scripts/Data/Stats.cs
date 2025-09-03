using UnityEngine;

public class Stats : ScriptableObject
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float MaxEnergy = 10f;
    public float MovementSpeedMultiplier = 1;
    public float DamageMultiplier = 1;
    public float DamageReduction = 0;
    public float AttackSpeed = 1;
    public float ResourceGainMultiplier = 1;
    public float DashForce = 1000f;
    public float IframeDuration = .2f;

    [Header("Vision")]
    public float Range = 4f;
    public float AlertRange = 6f;
    public float FieldOfView = 140f;
}
