using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{

    [BoxGroup("Debug")] public float Distance = 0;

    void Update()
    {
        if(PlayerBroadcast.Instance.Players[0] != null)
        {
            Distance = Vector3.Distance(PlayerBroadcast.Instance.Players[0].transform.position, transform.position);
        }        
    }

}
