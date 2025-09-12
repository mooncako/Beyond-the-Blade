using System;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryUtil
{
    // Generic deep-copy with a value cloner
    public static Dictionary<TKey, TValue> CloneToRuntime<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        Func<TValue, TValue> cloneValue)
    {
        var dst = new Dictionary<TKey, TValue>(source.Count);
        foreach (var (k, v) in source)
            dst[k] = cloneValue != null ? cloneValue(v) : v;
        return dst;
    }
}
