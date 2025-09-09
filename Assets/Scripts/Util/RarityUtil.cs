using UnityEngine;

public static class RarityUtil
{
    public static Color GetRarityColor(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Normal:
                return RARITYCOLOR.Normal;
            case SkillRarity.Rare:
                return RARITYCOLOR.Rare;
            case SkillRarity.Epic:
                return RARITYCOLOR.Epic;
            case SkillRarity.Legendary:
                return RARITYCOLOR.Legendary;
        }

        return Color.white;
    }
}
