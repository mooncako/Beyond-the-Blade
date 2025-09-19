using Unity.Mathematics;
using UnityEngine;


public static class ANIMKEY
{
    public const string Stagger = "Stagger";
    public const string Block = "Block";
    public const string IsDead = "IsDead";
    public const string Stance = "Stance";
}

public static class DIRECTORY
{

    public const string SettingsPath = "\\Settings.json";
}

public static class AUDIO
{
    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string SFXVolume = "SFXVolume";
    public const string AmbienceVolume = "AmbienceVolume";
    public const string VoiceVolume = "VoiceVolume";
}

public static class CHARGEVFX
{
    public const string Strength = "Strength";
    public const string TurbulenceTrigger = "TurbulenceTrigger";
}

public static class LIGHTING
{
    public const float MaxCandelaIntensity = 467083800;
    public const float MaxNitsIntensity = 10000000;
    public const float MaxAppliedBrightness = 40f;
}

public static class AVAILABLESKILLKEY
{
    public const int Attack = 0;
    public const int Parry = 1;
    public const int Ability = 2;
    public const int Execution = 3;
    public const int Dash = 4;
}

public static class RARITYCOLOR
{
    public static readonly Color Normal = new Color(198f / 255f, 250f / 255f, 162f / 255f);
    public static readonly Color Rare = new Color(87f / 255f, 164f / 255f, 253f / 255f);
    public static readonly Color Epic = new Color(179f / 255f, 125f / 255f, 248f / 255f);
    public static readonly Color Legendary = new Color(248f / 255f, 210f / 255f, 125f / 255f);
}

public static class LIMIT
{
    public static int MaxAbilityCount = 3;
}

public static class RARITYPOSSIBILITY
{
    
}