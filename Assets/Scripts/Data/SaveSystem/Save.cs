using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Save
{
    public Dictionary<string, Skill> PlayerSkillDatabase;
    public StatsData StatsData;
    public List<Dictionary<int, List<string>>> PlayerWeaponSkills;
    public int SoulShardCount;

    public Save(Dictionary<string, Skill> playerSkillDatabase, StatsData statsData, int soulShardCount, List<Dictionary<int, List<string>>> playerWeaponSkills)
    {
        PlayerSkillDatabase = playerSkillDatabase.CloneToRuntime(v => new Skill(v));
        StatsData = statsData;
        SoulShardCount = soulShardCount;
        PlayerWeaponSkills = new List<Dictionary<int, List<string>>>();
        for (int i = 0; i < playerWeaponSkills.Count; i++)
        {
            PlayerWeaponSkills.Add(playerWeaponSkills[i].CloneToRuntime(v => new List<string>(v)));
        }
    }
}
