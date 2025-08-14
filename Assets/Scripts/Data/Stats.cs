using UnityEngine;

public class Stats : ScriptableObject
{
    [Header("Health")]
    public float MaxHealth = 100f;

    [Header("Vision")]
    public float Range = 4f;
    public float AlertRange = 6f;
    public float FieldOfView = 140f;
}
