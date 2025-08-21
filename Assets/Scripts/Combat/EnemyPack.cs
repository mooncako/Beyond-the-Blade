using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyPack : MonoBehaviour, MMEventListener<EnemySpawnedEvent>, MMEventListener<EncounterStartEvent>
{
    [SerializeField, BoxGroup("Debug"), ReadOnly] private List<Health> _enemies = new List<Health>();
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _defaultTimer;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentTimer;
    private readonly HashSet<Health> _tracked = new();
    private WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    private bool _isTimerRunning = false;

    public void OnMMEvent(EnemySpawnedEvent e)
    {
        if (!_isTimerRunning)
        {
            StartCoroutine(TimerCO());
        }
        _enemies.Add(e.Health);
        _tracked.Add(e.Health);
        e.Health.OnDeath.AddListener(OnEnemyDeath);
    }

    public void OnMMEvent(EncounterStartEvent e)
    {
        _defaultTimer = e.Timer;
        _currentTimer = e.Timer;
        StopCoroutine(TimerCO());
        StartCoroutine(TimerCO());
    }

    private void OnEnable()
    {
        this.MMEventStartListening<EnemySpawnedEvent>();
        this.MMEventStartListening<EncounterStartEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<EnemySpawnedEvent>();
        this.MMEventStopListening<EncounterStartEvent>();
        foreach (var h in _tracked)
        {
            if (h != null)
            {
                h.OnDeath.RemoveListener(OnEnemyDeath);
            }
        }

        _tracked.Clear();

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
        _enemies.Remove(info.Health);

        if (_enemies.Count == 0)
        {
            RefreshTimer();
            StopCoroutine(TimerCO());
            _isTimerRunning = false;
            EnemyClearedEvent.Trigger();
        }
    }

    private void RefreshTimer()
    {
        _currentTimer = _defaultTimer;
    }
    
}
