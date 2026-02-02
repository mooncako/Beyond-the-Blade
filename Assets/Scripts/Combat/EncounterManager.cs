using System.Collections;
using System.Collections.Generic;
using Animancer;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class EncounterManager : MonoBehaviour,
    MMEventListener<EnemySpawnedEvent>,
    MMEventListener<EncounterStartEvent>,
    MMEventListener<EnemySpawnStoppedEvent>,
    MMEventListener<EnemyDeathEvent>
{
    [SerializeField, BoxGroup("Database")] private EncounterDatabaseSO _encounterDatabase;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<BaseEncounter> _onGoingEncounters = new List<BaseEncounter>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<BaseEncounter> _completedEncounters = new List<BaseEncounter>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _defaultTimer;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentTimer;
    private WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    private bool _isTimerRunning = false;

    private void Update()
    {
        for(int i = 0; i < _onGoingEncounters.Count; i++)
        {
            if(_onGoingEncounters[i].IsStarted)
            {
                if(!_onGoingEncounters[i].IsCompleted)
                {
                    _onGoingEncounters[i].OnUpdate();
                }
                else
                {
                    var completedEncounter = _onGoingEncounters[i];
                    _completedEncounters.Add(completedEncounter);
                    EncounterClearEvent.Trigger(completedEncounter.EncounterID);
                    _onGoingEncounters.RemoveAt(i);
                    

                }
            }
            
        }
    }


    public void OnMMEvent(EnemySpawnedEvent e)
    {
        for(int i = 0; i < _onGoingEncounters.Count; i++)
        {
            if(_onGoingEncounters[i].EncounterID == e.EncounterID)
            {
                if(_onGoingEncounters[i] is CombatEncounter ce)
                {
                    ce.AddEnemy(e.Health);
                }
            }
        }
        
    }

    public void OnMMEvent(EncounterStartEvent e)
    {

        // Check if encounter is already ongoing
        for(int i = 0; i < _onGoingEncounters.Count; i++)
        {
            if(_onGoingEncounters[i].EncounterID == e.EncounterID)
            {
                return;
            }
        }
        
        // Add encounter to ongoing encounters and start it
        if(_encounterDatabase.GetEncounter(e.EncounterID) != null)
        {
            switch(e.EncounterType)
            {
                case EncounterType.Combat:
                    var combatEncounter = new CombatEncounter(0, e.EncounterID, EnemySpawnModeType.Fixed);
                    _encounterDatabase.GetEncounter(e.EncounterID).Copy(combatEncounter);
                    combatEncounter.EnemyCount = e.EnemyCount;
                    combatEncounter.OnStart();
                    _onGoingEncounters.Add(combatEncounter);
                    break;

            }
        }
        else
        {
            Debug.LogError("Encounter with ID " + e.EncounterID + " not found in database.");
            return;
        }
        
    }

    public void OnMMEvent(EnemySpawnStoppedEvent e)
    {
        
    }

    public void OnMMEvent(EnemyDeathEvent e)
    {
        OnEnemyDeath(e.Info);
        for(int i = 0; i < _onGoingEncounters.Count; i++)
        {
            if(_onGoingEncounters[i] is CombatEncounter ce)
            {
                ce.RemoveEnemy(e.Info.Health);
            }
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<EnemySpawnedEvent>();
        this.MMEventStartListening<EncounterStartEvent>();
        this.MMEventStartListening<EnemySpawnStoppedEvent>();
        this.MMEventStartListening<EnemyDeathEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<EnemySpawnedEvent>();
        this.MMEventStopListening<EncounterStartEvent>();
        this.MMEventStopListening<EnemySpawnStoppedEvent>();
        this.MMEventStopListening<EnemyDeathEvent>();
    }

    private IEnumerator TimerCO()
    {
        _isTimerRunning = true;
        while (_currentTimer >= 0)
        {
            yield return _waitOneSec;
            _currentTimer--;
        }

        RefreshTimer();
        _isTimerRunning = false;
        EnemyClearedEvent.Trigger();
    }

    private void OnEnemyDeath(DamageInfo info)
    {
        var h = info.Health != null ? info.Health : info.Victim?.GetComponent<Health>();
        // if (h != null)
        // {
        //     _enemies.Remove(h);
        // }

        // if (_enemies.Count == 0)
        // {
        //     if (!_noExtraEnemies)
        //     {
        //         RefreshTimer();
        //         StopCoroutine(TimerCO());
        //         EnemyClearedEvent.Trigger();
        //     }
        // }

        // if (_noExtraEnemies)
        // {
        //     if(_enemies.Count == 0)
        //         LevelClearedEvent.Trigger(LevelManager.Instance.CurrentLevel.PossibleRewardType);
        // }
    }

    private void RefreshTimer()
    {
        _currentTimer = _defaultTimer;
    }
    
}
