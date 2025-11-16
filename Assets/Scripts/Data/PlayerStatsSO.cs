using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStatsSO : Stats
{
    public int GoldCount = 0;
    public int SoulShardCount = 0;

    public float BaseGoldDropRate = .3f;
    [ReadOnly] public float TempGoldDropRate = 0;
    [ShowInInspector, ReadOnly] public float GoldDropRate => BaseGoldDropRate + TempGoldDropRate;

    public override void Clear()
    {
        base.Clear();
        GoldCount = 0;
    }

}
