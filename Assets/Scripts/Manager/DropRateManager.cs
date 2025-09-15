using MoreMountains.Tools;
using UnityEngine;

public class DropRateManager : MMSingleton<DropRateManager>
{
    public float Normal = .4f;
    public float Rare = .7f;
    public float Epic = .9f;
    public float Legendary = 1f;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public Rarity GetRarity(float index)
    {
        return RarityUtil.GetRarity(Normal, Rare, Epic, Legendary, index);
    }
}
