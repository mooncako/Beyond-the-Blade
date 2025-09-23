using MoreMountains.Tools;
using UnityEngine;

public struct LoadSceneEvent
{
    public string SceneName;

    public LoadSceneEvent(string sceneName)
    {
        SceneName = sceneName;
    }

    public static LoadSceneEvent e;
    public static void Trigger(string sceneName)
    {
        e.SceneName = sceneName;
        MMEventManager.TriggerEvent(e);
    }
}
