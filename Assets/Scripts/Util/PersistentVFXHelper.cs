using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class PersistentVFXHelper : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] public List<VisualEffect> PersistentEffects = new List<VisualEffect>();

    public void StopPersistentEffects()
    {
        for (int i = 0; i < PersistentEffects.Count; i++)
        {
            PersistentEffects[i].Stop();
        }

        PersistentEffects.Clear();
    }
}
