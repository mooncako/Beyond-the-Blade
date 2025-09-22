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

    public PlayerSkillsSO DefaultPlayerSkillDatabase;
    public PlayerStatsSO DefaultPlayerStats;
    public List<AvailableSkillSO> DefaultWeaponSkills;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private string _currentSaveName; 

    void OnEnable()
    {
        this.MMEventStartListening<SaveEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SaveEvent>();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void OnMMEvent(SaveEvent e)
    {
        byte[] bytes;

        if (File.Exists($"{DIRECTORY.SavePath}{e.SaveName}.save"))
        {
            List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

            for (int i = 0; i < PlayerWeaponSkills.Count; i++)
            {
                playerWeaponSkills.Add(PlayerWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
            }

            Save save = new Save(PlayerSkillDatabase.SkillDict, PlayerStats.StatsData, playerWeaponSkills);

            var context = new SerializationContext();
            var resolver = new UnityReferenceResolver();
            context.Config.SerializationPolicy = SerializationPolicies.Everything;
            context.IndexReferenceResolver = resolver;
            bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary, context);

            if (!Directory.Exists(DIRECTORY.SavePath))
            {
                Directory.CreateDirectory(DIRECTORY.SavePath);
            }

            File.WriteAllBytes($"{DIRECTORY.SavePath}{e.SaveName}.save", bytes);
        }
        else
        {
            List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

            for (int i = 0; i < DefaultWeaponSkills.Count; i++)
            {
                playerWeaponSkills.Add(DefaultWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
            }

            Save save = new Save(DefaultPlayerSkillDatabase.SkillDict, DefaultPlayerStats.StatsData, playerWeaponSkills);

            var context = new SerializationContext();
            var resolver = new UnityReferenceResolver();
            context.Config.SerializationPolicy = SerializationPolicies.Everything;
            context.IndexReferenceResolver = resolver;
            bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary, context);

            if (!Directory.Exists(DIRECTORY.SavePath))
            {
                Directory.CreateDirectory(DIRECTORY.SavePath);
            }



            File.WriteAllBytes($"{DIRECTORY.SavePath}{e.SaveName}.save", bytes);

            // Load function
        }

        _currentSaveName = e.SaveName;

    }

    public void OnMMEvent(LoadEvent e)
    {
        byte[] bytes = File.ReadAllBytes($"{DIRECTORY.SavePath}{e.SaveName}.save");
        var context = new DeserializationContext();
        context.IndexReferenceResolver = new UnityReferenceResolver();
        Save save = SerializationUtility.DeserializeValue<Save>(bytes, DataFormat.Binary, context);
        PlayerSkillDatabase.SkillDict = save.PlayerSkillDatabase.CloneToRuntime(v => new Skill(v));
        PlayerStats.CopyValue(save.StatsData);
        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            PlayerWeaponSkills[i].SkillDict = save.PlayerWeaponSkills[i].CloneToRuntime(v => new List<string>(v));
        }

        _currentSaveName = e.SaveName;
    }

    [Button]
    private void TestSave()
    {
        byte[] bytes;

        if (File.Exists($"{DIRECTORY.SavePath}Save01.save"))
        {
            List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

            for (int i = 0; i < PlayerWeaponSkills.Count; i++)
            {
                playerWeaponSkills.Add(PlayerWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
            }

            Save save = new Save(PlayerSkillDatabase.SkillDict, PlayerStats.StatsData, playerWeaponSkills);

            
            bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary);
        }
        else
        {
            List<Dictionary<int, List<string>>> playerWeaponSkills = new List<Dictionary<int, List<string>>>();

            for (int i = 0; i < DefaultWeaponSkills.Count; i++)
            {
                playerWeaponSkills.Add(DefaultWeaponSkills[i].SkillDict.CloneToRuntime(v => new List<string>(v)));
            }

            Save save = new Save(DefaultPlayerSkillDatabase.SkillDict, DefaultPlayerStats.StatsData, playerWeaponSkills);
            bytes = SerializationUtility.SerializeValue(save, DataFormat.Binary);
        }
        
        if (!Directory.Exists(DIRECTORY.SavePath))
        {
            Directory.CreateDirectory(DIRECTORY.SavePath);
        }

        File.WriteAllBytes($"{DIRECTORY.SavePath}Save01.save", bytes);
    }

    [Button]
    private void TestLoad()
    {
        byte[] bytes = File.ReadAllBytes($"{DIRECTORY.SavePath}Save01.Save");
        Save save = SerializationUtility.DeserializeValue<Save>(bytes, DataFormat.Binary);
        PlayerSkillDatabase.SkillDict = save.PlayerSkillDatabase.CloneToRuntime(v => new Skill(v));
        PlayerStats.CopyValue(save.StatsData);
        for (int i = 0; i < PlayerWeaponSkills.Count; i++)
        {
            PlayerWeaponSkills[i].SkillDict = save.PlayerWeaponSkills[i].CloneToRuntime(v => new List<string>(v));
        }
    }

    
}
