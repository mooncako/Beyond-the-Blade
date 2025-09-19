using System.Collections.Generic;
using System.IO;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class SaveLoadManager : MMSingleton<SaveLoadManager>,
    MMEventListener<SaveEvent>,
    MMEventListener<LoadEvent>
{
    public PlayerSkillsSO PlayerSkillDatabase;
    public PlayerStatsSO PlayerStats;
    public List<AvailableSkillSO> PlayerWeaponSkills;


    void OnEnable()
    {
        this.MMEventStartListening<SaveEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SaveEvent>();
    }

    public void OnMMEvent(SaveEvent e)
    {
        List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            playerWeaponSkills.Add(PlayerWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
        }

        Save save = new Save(PlayerSkillDatabase.SkillDict, PlayerStats.StatsData, playerWeaponSkills);
        byte[] bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary);
        File.WriteAllBytes($"{Application.persistentDataPath}\\{e.SaveName}", bytes);
    }

    public void OnMMEvent(LoadEvent e)
    {
        byte[] bytes = File.ReadAllBytes($"{Application.persistentDataPath}\\{e.SaveName}");
        Save save = SerializationUtility.DeserializeValue<Save>(bytes, DataFormat.Binary);
        PlayerSkillDatabase.SkillDict = save.PlayerSkillDatabase.CloneToRuntime(v => new Skill(v));
        PlayerStats.CopyValue(save.StatsData);
        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            PlayerWeaponSkills[i].SkillDict = save.PlayerWeaponSkills[i].CloneToRuntime(v => new List<string>(v));
        }
    }

    [Button]
    private void TestSave()
    {
        List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            playerWeaponSkills.Add(PlayerWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
        }

        Save save = new Save(PlayerSkillDatabase.SkillDict, PlayerStats.StatsData, playerWeaponSkills);
        byte[] bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary);
        File.WriteAllBytes($"{Application.persistentDataPath}\\Test", bytes);
    }

    [Button]
    private void TestLoad()
    {
        byte[] bytes = File.ReadAllBytes($"{Application.persistentDataPath}\\Test");
        Save save = SerializationUtility.DeserializeValue<Save>(bytes, DataFormat.Binary);
        PlayerSkillDatabase.SkillDict = save.PlayerSkillDatabase.CloneToRuntime(v => new Skill(v));
        PlayerStats.CopyValue(save.StatsData);
        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            PlayerWeaponSkills[i].SkillDict = save.PlayerWeaponSkills[i].CloneToRuntime(v => new List<string>(v));
        }
    }

    
}
