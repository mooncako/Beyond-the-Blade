using UnityEngine;
using System.Collections.Generic;

public static class AnimatorExtensions
{
    private static Dictionary<string, AnimationClip> _stateToClipCache 
        = new Dictionary<string, AnimationClip>();

    /// <summary>
    /// Replace the clip used by a specific Animator state.
    /// </summary>
    public static void OverrideClipForState(this Animator animator, string stateName, AnimationClip newClip)
    {
        if (animator.runtimeAnimatorController == null)
            return;

        // Ensure AnimatorOverrideController is set up
        AnimatorOverrideController overrideController;
        if (animator.runtimeAnimatorController is AnimatorOverrideController existingOverride)
        {
            overrideController = existingOverride;
        }
        else
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }

        // Find the default clip for this state
        if (!_stateToClipCache.TryGetValue(stateName, out var originalClip))
        {
            foreach (var clip in overrideController.animationClips)
            {
                // State names in Mecanim default to the same as their clip names 
                // unless manually renamed. Adjust if your setup differs.
                if (clip.name == stateName)
                {
                    originalClip = clip;
                    _stateToClipCache[stateName] = originalClip;
                    break;
                }
            }
        }

        if (originalClip != null)
        {
            overrideController[originalClip] = newClip;
        }
        else
        {
            Debug.LogWarning($"No default clip found for state '{stateName}'. Check naming.");
        }
    }
}

