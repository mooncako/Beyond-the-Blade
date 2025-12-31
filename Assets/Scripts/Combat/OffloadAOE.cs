using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AOEApplier))]
public class OffloadAOE : AreaOfEffect, IPoolable
{
    [HideInInspector] public UnityEvent OnAOETriggered;
    [HideInInspector] public ObjectPool ObjectPool;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(ObjectPool != null)
        {
            ObjectPool.Return(gameObject);
        }
    }

    void Update()
    {
        
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (_enabled)
        {
            if (_aoeApplier.GetDamagedEntities(_range.AreaType, transform.position, _enemyMask).Count > 0)
            {
                List<GameObject> gameObjects = _aoeApplier.GetDamagedEntities(_range.AreaType, transform.position, _enemyMask);
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].GetComponent<Health>().Damage(new DamageInfo(_damage, gameObjects[i], gameObjects[i].GetComponent<Health>(), gameObject, DamageType.Regular));
                }
                _enabled = false;
                OnAOETriggered.Invoke();
            }
        }
    }


    public void OnPoolGet()
    {
        
    }

    public void OnPoolReturn()
    {
        OnAOETriggered.RemoveAllListeners();
    }
}
