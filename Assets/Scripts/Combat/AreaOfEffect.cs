using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AOEApplier))]
public class AreaOfEffect : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] protected MeshFilter _meshFilter;
    [SerializeField, BoxGroup("References")] protected MeshRenderer _meshRenderer;
    [SerializeField, BoxGroup("References")] protected MeshCollider _meshCollider;
    [SerializeField, BoxGroup("References")] protected AOEApplier _aoeApplier;
    [SerializeField, BoxGroup("Settings")] protected LayerMask _enemyMask;
    [SerializeField, BoxGroup("Debug")] protected SkillRange _range;
    [SerializeField, BoxGroup("Debug")] protected float _damage;
    [SerializeField, BoxGroup("Debug")] protected bool _enabled = false;

    protected virtual void OnValidate()
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();
        if (_aoeApplier == null) _aoeApplier = GetComponent<AOEApplier>();
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        
    }

    public virtual void AssignData(Skill skill)
    {
        _range = skill.SkillRange;
        _damage = skill.Damage;
    }

    [Button]
    public virtual void Generate()
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

    public virtual void OnPoolGet()
    {
        
    }

    public virtual void OnPoolReturn()
    {
        
    }
}
