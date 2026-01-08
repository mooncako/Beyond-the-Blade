using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBuilder : MonoBehaviour,
    MMEventListener<BuildNavMeshEvent>
{
    [SerializeField, BoxGroup("References")] private NavMeshSurface _navMeshSurface;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _hasTriggered = false;

    void OnValidate()
    {
        if(_navMeshSurface == null) _navMeshSurface = GetComponent<NavMeshSurface>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<BuildNavMeshEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<BuildNavMeshEvent>();
    }

    [Button]
    private void Build(bool forceBuild)
    {
        if(forceBuild)
        {
            _navMeshSurface.BuildNavMesh();
        }
        else
        {
            if(!_hasTriggered)
            {
                _hasTriggered = true;
                _navMeshSurface.BuildNavMesh();
            }
        }
        
    }

    public void OnMMEvent(BuildNavMeshEvent e)
    {
        Build(e.ForceBuild);
    }
}
