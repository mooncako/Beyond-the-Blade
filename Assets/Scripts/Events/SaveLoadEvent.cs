using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public struct SaveEvent
{
    public string SaveName;
    // Need to add stuffs related to the meta progression as well as story

    public SaveEvent(string saveName)
    {
        SaveName = saveName;
    }

    public static SaveEvent e;

    public static void Trigger(string saveName)
    {
        e.SaveName = saveName;
        MMEventManager.TriggerEvent(e);
    }
}

public struct LoadEvent
{
    public string SaveName;
    public LoadEvent(string saveName)
    {
        SaveName = saveName;
    }

    public static LoadEvent e;
    public static void Trigger(string saveName)
    {
        e.SaveName = saveName;
        MMEventManager.TriggerEvent(e);
    }
}
