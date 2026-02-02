using Sirenix.OdinInspector;
using UnityEngine;

public class EncounterSetting : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private string _encounterID;
    public string EncounterID => _encounterID; 

    public void StartEncounter(EncounterType encounterType, int enemyCount)
    {
        EncounterStartEvent.Trigger(enemyCount, _encounterID, encounterType);
    }
}
