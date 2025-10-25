using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "VFXDatabase", menuName = "BytheBlade/VFXDatabaseSO")]
public class VFXPrefabDatabaseSO : SerializedScriptableObject
{
    public Dictionary<string, GameObject> VFXDatabase = new Dictionary<string, GameObject>();
    public List<GameObject> VFXPrefabs = new List<GameObject>();

    [Button]
    private void PopulateList()
    {
        VFXPrefabs.Clear();
        foreach (KeyValuePair<string, GameObject> entry in VFXDatabase)
        {
            VFXPrefabs.Add(entry.Value);
        }
    }
}
