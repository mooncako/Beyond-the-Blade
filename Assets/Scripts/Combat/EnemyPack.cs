using System.Collections;
using System.Collections.Generic;
using Animancer;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyPack : MonoBehaviour,
    MMEventListener<EnemySpawnedEvent>,
    MMEventListener<EncounterStartEvent>,
    MMEventListener<EnemySpawnStoppedEvent>,
    MMEventListener<EnemyDeathEvent>
{
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<Health> _enemies = new List<Health>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _defaultTimer;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentTimer;
    private WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    private bool _isTimerRunning = false;
    [ShowInInspector, ReadOnly] private bool _noExtraEnemies = false;

    public void OnMMEvent(EnemySpawnedEvent e)
    {
        if (!_isTimerRunning)
        {
            StartCoroutine(TimerCO());
        }

        if(!_enemies.Contains(e.Health))
            _enemies.Add(e.Health);
    }

    public void OnMMEvent(EncounterStartEvent e)
    {
        _defaultTimer = e.Timer;
        _currentTimer = e.Timer;
        StopCoroutine(TimerCO());
        StartCoroutine(TimerCO());
    }

    public void OnMMEvent(EnemySpawnStoppedEvent e)
    {
        _noExtraEnemies = true;
    }

    public void OnMMEvent(EnemyDeathEvent e)
    {
        OnEnemyDeath(e.Info);
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
        if (h != null)
        {
            _enemies.Remove(h);
        }

        if (_enemies.Count == 0)
        {
            if (!_noExtraEnemies)
            {
                RefreshTimer();
                StopCoroutine(TimerCO());
                EnemyClearedEvent.Trigger();
            }
        }

        if (_noExtraEnemies)
        {
            if(_enemies.Count == 0)
                LevelClearedEvent.Trigger(LevelManager.Instance.CurrentLevel.PossibleRewardType);
        }
    }

    private void RefreshTimer()
    {
        _currentTimer = _defaultTimer;
    }
    
}
