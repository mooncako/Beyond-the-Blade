using UnityEngine;



public static class STATE
{
    public const int Idle = 0;
    public const int Moving = 1;
    public const int Attacking = 2;
    public const int Charging = 3;
    public const int Dead = 4;
    public const int Parrying = 5;
    public const int Staggered = 6;
    public const int Muso = 7;
    public const int Sheathe = 8;
}

public static class ANIMKEY
{
    public const string Stagger = "Stagger";
    public const string Block = "Block";
    public const string IsDead = "IsDead";
    public const string Stance = "Stance";
}

public static class DIRECTORY
{
    //Really Important -by Xwen
    public const string SavePath = "\\Save";
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

