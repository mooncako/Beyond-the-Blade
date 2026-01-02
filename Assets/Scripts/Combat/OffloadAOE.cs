using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class OffloadAOE : AreaOfEffect
{
    [HideInInspector] public UnityEvent OnAOETriggered;
    [HideInInspector] public ObjectPool ObjectPool;

    protected override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
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


    public override void OnPoolGet()
    {
        
    }

    public override void OnPoolReturn()
    {
        OnAOETriggered.RemoveAllListeners();
    }
}
