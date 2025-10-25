using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Animancer.TransitionLibraries;
using Animancer;
[CreateAssetMenu(fileName = "LocomotionAnimation", menuName = "BytheBlade/LocomotionAnimation")]
public class LocomotionAnimationSO : SerializedScriptableObject
{
    public ClipTransition Idle;
    public ClipTransition Run;
    public TransitionLibraryAsset TransitionLibrary;
}
