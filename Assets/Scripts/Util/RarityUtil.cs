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
}
