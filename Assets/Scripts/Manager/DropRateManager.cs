using MoreMountains.Tools;
using UnityEngine;

public class DropRateManager : MMSingleton<DropRateManager>
{
    [Header("Rarity Rate")]
    public float Normal = .4f;
    public float Rare = .7f;
    public float Epic = .9f;
    public float Legendary = 1f;

    [Header("Drop Rate")]
    public float Skill = .2f;
    public float Modifier = .4f;
    public float Ability = .6f;
    public float Item = 1f;

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
