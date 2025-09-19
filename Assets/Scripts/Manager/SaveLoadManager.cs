using System.Collections.Generic;
using MoreMountains.Tools;
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
        
    }

    public void OnMMEvent(LoadEvent e)
    {
        
    }
}
