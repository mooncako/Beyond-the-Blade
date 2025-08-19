using System;
using UnityEngine;

[Serializable]
public class PoolConfig
{
    [Tooltip("Prefab to pool")]
    public GameObject prefab;

    [Tooltip("Initial number of objects to create")]
    [Range(1, 100)]
    public int initialSize = 10;

    [Tooltip("Maximum number of objects in pool (0 = unlimited)")]
    [Range(0, 500)]
    public int maxSize = 50;

    [Tooltip("Can pool expand beyond initial size")]
    public bool canExpand = true;
}