using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField, BoxGroup("Debug")] private Transform _player;

    [BoxGroup("Debug")] public float Distance = 0;

    void Update()
    {
        if(_player != null)
        {
            Distance = Vector3.Distance(_player.position, transform.position);
        }        
    }

    void OnEnable()
    {
        _player = GameObject.Find("Player").transform;
    }
}
