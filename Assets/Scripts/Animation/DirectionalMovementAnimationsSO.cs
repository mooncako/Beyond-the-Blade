using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DirectionalMovementAnimations", menuName = "Animation/Directional Movement Animations")]
public class DirectionalMovementAnimationsSO : ScriptableObject
{
    [Header("Movement Animations")]
    [SerializeField, Required] private AnimationClip _idle;
    [SerializeField, Required] private AnimationClip _runForward;
    [SerializeField, Required] private AnimationClip _runBackward;
    [SerializeField, Required] private AnimationClip _strafeLeft;
    [SerializeField, Required] private AnimationClip _strafeRight;

    [Header("Mixer Settings")]
    [SerializeField] private float _fadeDuration = 0.25f;
    
    public AnimationClip Idle => _idle;
    public AnimationClip RunForward => _runForward;
    public AnimationClip RunBackward => _runBackward;
    public AnimationClip StrafeLeft => _strafeLeft;
    public AnimationClip StrafeRight => _strafeRight;
    public float FadeDuration => _fadeDuration;


    public MixerTransition2D CreateDirectionalMixer()
    {
        var mixer = new MixerTransition2D();
        mixer.FadeDuration = _fadeDuration;
        
     
   
   
        mixer.Animations = new UnityEngine.Object[]
        {
            _idle,          // Index 0 - Center (Idle)
            _runForward,    // Index 1 - Forward
            _runBackward,   // Index 2 - Backward
            _strafeLeft,    // Index 3 - Left
            _strafeRight    // Index 4 - Right
        };
        

        mixer.Thresholds = new Vector2[]
        {
            Vector2.zero,       // Index 0 - Center (0,0) - Idle
            Vector2.up,         // Index 1 - Forward (0,1)
            Vector2.down,       // Index 2 - Backward (0,-1)
            Vector2.left,       // Index 3 - Left (-1,0)
            Vector2.right       // Index 4 - Right (1,0)
        };
        
  
        mixer.Type = MixerTransition2D.MixerType.Directional;

        var parameterXAsset = ScriptableObject.CreateInstance<StringAsset>();
        parameterXAsset.name = "Right";
        var parameterYAsset = ScriptableObject.CreateInstance<StringAsset>();
        parameterYAsset.name = "Forward";
        
        mixer.ParameterNameX = parameterXAsset;
        mixer.ParameterNameY = parameterYAsset;
        
        return mixer;
    }

    private void OnValidate()
    {

        if (_idle == null) Debug.LogWarning("Idle animation is not assigned!", this);
        if (_runForward == null) Debug.LogWarning("Run Forward animation is not assigned!", this);
        if (_runBackward == null) Debug.LogWarning("Run Backward animation is not assigned!", this);
        if (_strafeLeft == null) Debug.LogWarning("Strafe Left animation is not assigned!", this);
        if (_strafeRight == null) Debug.LogWarning("Strafe Right animation is not assigned!", this);
    }
}
