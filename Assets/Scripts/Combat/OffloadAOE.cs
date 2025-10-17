using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AOEApplier))]
public class OffloadAOE : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] private MeshFilter _meshFilter;
    [SerializeField, BoxGroup("References")] private MeshRenderer _meshRenderer;
    [SerializeField, BoxGroup("References")] private MeshCollider _meshCollider;
    [SerializeField, BoxGroup("References")] private AOEApplier _aoeApplier;
    [SerializeField, BoxGroup("Settings")] private LayerMask _enemyMask;
    [SerializeField, BoxGroup("Debug")] private SkillRange _range;
    [SerializeField, BoxGroup("Debug")] private float _damage;
    [SerializeField, BoxGroup("Debug")] private bool _enabled = false;
    [HideInInspector] public UnityEvent OnAOETriggered;
    [HideInInspector] public ObjectPool ObjectPool;


    void OnValidate()
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();
        if (_aoeApplier == null) _aoeApplier = GetComponent<AOEApplier>();
    }

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

    [Button]
    public void Generate()
    {
        switch (_range.AreaType)
        {
            case SkillAreaType.Arc:
                _meshFilter.sharedMesh = AOEGenerator.BuildSector(_range.X, _range.Z);
                break;
            case SkillAreaType.Circle:
                _meshFilter.sharedMesh = AOEGenerator.BuildCircle(_range.X);
                break;
            case SkillAreaType.Box:
                _meshFilter.sharedMesh = AOEGenerator.BuildRectangle(_range.X, _range.Y);
                break;
            case SkillAreaType.Cone:
                _meshFilter.sharedMesh = AOEGenerator.BuildSector(_range.X, _range.Z);
                break;
        }
        _meshCollider.sharedMesh = _meshFilter.sharedMesh;
        _meshCollider.isTrigger = true;
        _aoeApplier.X = _range.X;
        _aoeApplier.Y = _range.Y;
        _aoeApplier.Z = _range.Z;
        _enabled = true;
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
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

    public void AssignData(Skill skill)
    {
        _range = skill.SkillRange;
        _damage = skill.Damage;
    }

    public void OnPoolGet()
    {
        
    }

    public void OnPoolReturn()
    {
        OnAOETriggered.RemoveAllListeners();
    }
}
