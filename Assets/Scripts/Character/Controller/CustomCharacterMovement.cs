using CharacterMovement;
using UnityEngine;

public class CustomCharacterMovement : CharacterMovement3D
{
    public float CurrentSpeedMultiplier { get; set; } = 1f;

    public void Teleport(Vector3 position)
    {
        transform.position = position;
        Rigidbody.position = position;
    }

    public void KnockBack(Transform instigator, float KnockbackForce = 2000f)
    {
        Vector3 knockBackDirection = transform.position - instigator.position;
        Rigidbody.AddForce(knockBackDirection.normalized * KnockbackForce);
    }

    public void ResetSpeed() => CurrentSpeedMultiplier = 1f;
    public void SetSpeedMultiplier(float multiplier)
    {
        CurrentSpeedMultiplier = multiplier;
    }
}
