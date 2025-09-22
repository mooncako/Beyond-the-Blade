using MoreMountains.Tools;
using UnityEngine;

public class StartGameManager : MMSingleton<StartGameManager>
{
    public void StartGame(string saveName)
    {
        SaveEvent.Trigger(saveName);
    }
}
