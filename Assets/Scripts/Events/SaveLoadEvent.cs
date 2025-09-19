using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public struct SaveEvent
{
    public string SaveName;
    public PlayerSkillsSO PlayerSkillDatabase;
    public PlayerStatsSO PlayerStats;
    public List<AvailableSkillSO> PlayerWeaponSkills;
    // Need to add stuffs related to the meta progression as well as story

    public SaveEvent(string saveName, PlayerSkillsSO playerSkillDatabase, PlayerStatsSO playerStats, List<AvailableSkillSO> playerWeaponSkills)
    {
        SaveName = saveName;
        PlayerSkillDatabase = playerSkillDatabase;
        PlayerStats = playerStats;
        PlayerWeaponSkills = playerWeaponSkills;
    }

    public static SaveEvent e;

    public static void Trigger(string saveName, PlayerSkillsSO playerSkillDatabase, PlayerStatsSO playerStats, List<AvailableSkillSO> playerWeaponSkills)
    {
        e.SaveName = saveName;
        e.PlayerSkillDatabase = playerSkillDatabase;
        e.PlayerStats = playerStats;
        e.PlayerWeaponSkills = playerWeaponSkills;
        MMEventManager.TriggerEvent(e);
    }
}

public struct LoadEvent
{
    public string SaveName;
    public LoadEvent(string saveName)
    {
        SaveName = saveName;
    }

    public static LoadEvent e;
    public static void Trigger(string saveName)
    {
        e.SaveName = saveName;
        MMEventManager.TriggerEvent(e);
    }
}
