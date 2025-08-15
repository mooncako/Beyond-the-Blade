using UnityEngine;

public static class HitStop
{
    public static void Begin(Animator animator, string animationSpeedKey)
    {
        animator.SetFloat(animationSpeedKey, 0);
    }

    public static void Stop(Animator animator, string animationSpeedKey)
    {
        animator.SetFloat(animationSpeedKey, 1);
    }
}
