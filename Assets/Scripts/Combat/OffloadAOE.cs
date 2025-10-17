using Sirenix.OdinInspector;
using UnityEngine;

public class OffloadAOE : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private MeshFilter _meshFilter;
    [SerializeField, BoxGroup("References")] private MeshRenderer _meshRenderer;
    [SerializeField, BoxGroup("References")] private MeshCollider _meshCollider;
    [SerializeField, BoxGroup("Debug")] private SkillRange _range;
    [SerializeField, BoxGroup("Debug")] private float _damage;
    [SerializeField, BoxGroup("Debug")] private bool _enabled = false;


    void OnValidate()
    {
        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
        if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();
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
    }

    public void AssignData(Skill skill)
    {
        _range = skill.SkillRange;
        _damage = skill.Damage;
    }
}
