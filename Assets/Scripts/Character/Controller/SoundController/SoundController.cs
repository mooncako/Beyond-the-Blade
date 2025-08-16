using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] protected FMODUnity.EventReference _attackSFX;
    [SerializeField] protected FMODUnity.EventReference _footstepSFX;
    [SerializeField] protected FMODUnity.EventReference _movementSFX;

    public virtual void PlayFootstep()
    {

    }

    public virtual void PlayMovementSound()
    {

    }

    public virtual void PlayAttackSFX()
    {

    }
}
