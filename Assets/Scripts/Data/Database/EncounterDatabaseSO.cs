using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "EncounterDatabase", menuName = "BytheBlade/EncounterDatabase")]
public class EncounterDatabaseSO : SerializedScriptableObject
{
    [SerializeField, BoxGroup("Encounters")] public Dictionary<string, CombatEncounter> CombatEncounterDict = new Dictionary<string, CombatEncounter>();

    public BaseEncounter GetEncounter(string encounterID)
    {
        for (int i = 0; i < CombatEncounterDict.Count; i++)
        {
            if (CombatEncounterDict.ElementAt(i).Key == encounterID)
            {
                return CombatEncounterDict.ElementAt(i).Value;
            }
        }

        return null;
    }
}
