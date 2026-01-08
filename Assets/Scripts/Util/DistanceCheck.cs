using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class DistanceCheck : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float _distanceThreshold = 1.5f;

    public bool GetCheckResult()
    {
        if(PlayerBroadcast.Instance.Players[0] != null)
        {
           return Vector3.Distance(PlayerBroadcast.Instance.Players[0].transform.position, transform.position) <= _distanceThreshold;
        }
        else
        {
            return false;
        }
    }

}
