using Sirenix.OdinInspector;
using UnityEngine;

public class BaseEncounter
{
    public string EncounterID;
    public bool IsStarted = false;
    [ShowInInspector]public bool IsCompleted => CheckCompletion();

    protected virtual bool CheckCompletion()
    {
        return false;
    }

    public virtual void OnStart()
    {
        IsStarted = true;
    }

    public virtual void OnUpdate()
    {
        
    }

    public virtual void Copy(BaseEncounter other)
    {
        
    }
}
