using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;
public class AnimancerCharacterMovement : MonoBehaviour
{

    public AnimancerComponent animancer;
    [SerializeField] public ClipTransition Idle;
    [SerializeField] public ClipTransition Run;
    [SerializeField] private TransitionLibraryAsset _locomotionLibrary;
    private Rigidbody _rigidbody;
    
    void Start()
    {
        animancer.Transitions = _locomotionLibrary;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_rigidbody.linearVelocity.magnitude > 0.1f)
        {
            animancer.Play(Run);
        }
        else
        {
            animancer.Play(Idle);
        }
        
        // if(forward > 0)
        // {
        //     animancer.Play(Run);
        // }
        // else if (forward == 0)
        // {
        //     animancer.Play(Idle);
        // }
        
    }
}
