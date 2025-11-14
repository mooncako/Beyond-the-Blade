using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;


[RequireComponent(typeof(DontDestroy))]
public class PlayerBroadcast : MMSingleton<PlayerBroadcast>,
    MMEventListener<PlayerInitializedEvent>
{
    [SerializeField, BoxGroup("Debug"), ReadOnly] public List<PlayerController> Players = new List<PlayerController>();

    void OnEnable()
    {
        this.MMEventStartListening<PlayerInitializedEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerInitializedEvent>();
    }


    public void OnMMEvent(PlayerInitializedEvent e)
    {
        if(Players.Contains(e.Player)) return;

        Players.Add(e.Player);
    }
}
