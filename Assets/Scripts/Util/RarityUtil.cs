using UnityEngine;

public static class RarityUtil
{
    public static Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Normal:
                return RARITYCOLOR.Normal;
            case Rarity.Rare:
                return RARITYCOLOR.Rare;
            case Rarity.Epic:
                return RARITYCOLOR.Epic;
            case Rarity.Legendary:
                return RARITYCOLOR.Legendary;
        }

        return Color.white;
    }

    public static Rarity GetRarity(float normal, float rare, float epic, float legendary, float index)
    {
        if (index <= normal)
        {
            return Rarity.Normal;
        }
        else if (index <= rare)
        {
            return Rarity.Rare;
        }
        else if (index <= epic)
        {
            return Rarity.Epic;
        }
        else
        {
            return Rarity.Legendary;
        }
    }
}
