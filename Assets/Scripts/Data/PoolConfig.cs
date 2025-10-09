using System;
using UnityEngine;

[Serializable]
public class PoolConfig
{
    [Tooltip("Prefab to pool")]
    public GameObject Prefab;

    [Tooltip("Initial number of objects to create")]
    [Range(1, 100)]
    public int InitialSize = 10;

    [Tooltip("Maximum number of objects in pool (0 = unlimited)")]
    [Range(0, 500)]
    public int MaxSize = 50;

    [Tooltip("Can pool expand beyond initial size")]
    public bool CanExpand = true;

    public PoolConfig(GameObject prefab, int initialSize, int maxSize, bool canExpand)
    {
        Prefab = prefab;
        InitialSize = initialSize;
        MaxSize = maxSize;
        CanExpand = canExpand;
    }
}