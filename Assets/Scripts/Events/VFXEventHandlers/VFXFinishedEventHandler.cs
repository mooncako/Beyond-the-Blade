using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXFinishedEventHandler : VFXOutputEventAbstractHandler
{
    [HideInInspector] public UnityEvent OnSpawnFinished;
    [SerializeField, BoxGroup("References")] private VisualEffect _vfx;
    public override bool canExecuteInEditor => true;

    void OnValidate()
    {
        if(_vfx == null) _vfx = GetComponent<VisualEffect>();
    }

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        _vfx.Stop();
        OnSpawnFinished.Invoke();
    }
}
