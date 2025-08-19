using Animancer;
using UnityEngine;

public class AnimancerCharacterMovement : MonoBehaviour
{
    public float forward;
    public float right;
    public AnimancerComponent animancer;
    [SerializeField] public AnimationClip Idle;
    [SerializeField] public AnimationClip Run;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(forward > 0)
        {
            animancer?.Play(Idle);
        }
        else if (forward == 0)
        {
            animancer?.Play(Run);
        }
        
    }
}
