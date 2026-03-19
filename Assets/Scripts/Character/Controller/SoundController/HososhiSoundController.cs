using MoreMountains.Tools;
using UnityEngine;

public class HososhiSoundController : SoundController,
    MMEventListener<OnZeroDamageEvent>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<OnZeroDamageEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<OnZeroDamageEvent>();
    }

    public void OnMMEvent(OnZeroDamageEvent e)
    {
        if(e.Source != gameObject) return;
        
        // For Ed: Play block sound effect
    }
}
