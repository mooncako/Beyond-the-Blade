using UnityEngine;

public class PlayerSoundController : SoundController
{
    [Header("SFX")]
    [SerializeField] private FMODUnity.EventReference _heavyAttackSFX;
    [SerializeField] private FMODUnity.EventReference _musoAttackSFX;
    [SerializeField] private FMODUnity.EventReference _weakParrySFX;
    [SerializeField] private FMODUnity.EventReference _perfectParrySFX;
    [SerializeField] private FMODUnity.EventReference _sheatheSFX;
    [SerializeField] private FMODUnity.EventReference _musoActivatedSFX;
    [SerializeField] private FMODUnity.EventReference _musoExitSFX;
    [SerializeField] private FMODUnity.EventReference _musoLoopSFX;
    [SerializeField] private FMODUnity.EventReference _musoChargeGainedSFX;

    public override void PlayFootstep()
    {

    }

    public override  void PlayMovementSound()
    {

    }

     public override  void PlayAttackSFX()
    {

        FMODUnity.RuntimeManager.PlayOneShot(_attackSFX, transform.position);
        //create instance
        //attach to object
        //play the instance
        //release 
        // _attackInstance = FMODUnity.RuntimeManager.CreateInstance(_attackSFX);
        // // int randomIndex = Random.Range(0, 3);
        // // _attackInstance.setParameterByName("PlayerAttack", randomIndex);
        // RuntimeManager.AttachInstanceToGameObject(_attackInstance, gameObject, GetComponent<Rigidbody>());
        // _attackInstance.start();
        // _attackInstance.release();
        
    }
}
