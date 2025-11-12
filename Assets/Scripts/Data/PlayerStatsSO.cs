using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsSO : Stats
{
    public int GoldCount = 0;
    public int SoulShardCount = 0;

    public override void Clear()
    {
        base.Clear();
        GoldCount = 0;
    }

}
