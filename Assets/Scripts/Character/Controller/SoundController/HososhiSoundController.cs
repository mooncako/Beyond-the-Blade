using MoreMountains.Tools;
using UnityEngine;

public class HososhiSoundController : SoundController
{
    public override void OnMMEvent(OnZeroDamageEvent e)
    {
        if(e.Source != gameObject) return;

        // For Ed: Play block sound effect
    }
}
