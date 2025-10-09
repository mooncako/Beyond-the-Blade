using Animancer;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXFinishedEventHandler : VFXOutputEventAbstractHandler
{
    [HideInInspector] public UnityEvent OnSpawnFinished;
    public override bool canExecuteInEditor => true;

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        OnSpawnFinished.Invoke();
    }
}
