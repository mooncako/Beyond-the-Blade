using sc.splines.spawner.runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(SplineSpawner))]
public class GameplayObjectSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SplineSpawner _splineSpawner;

    void OnValidate()
    {
        if (_splineSpawner == null)
        {
            _splineSpawner = GetComponent<SplineSpawner>();
            _splineSpawner.respawningMode = SplineSpawner.RespawningMode.None;
        }
    }

    public void Respawn()
    {
        _splineSpawner.Respawn();
    }


}
