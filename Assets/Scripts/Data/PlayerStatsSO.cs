using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsSO : Stats
{
    [Header("Player Specific Stats")]
    public float MaxEnergy = 10;
}
