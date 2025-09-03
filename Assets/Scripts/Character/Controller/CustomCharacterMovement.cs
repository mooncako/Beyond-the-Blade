using Animancer;
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

    public void Teleport(Transform transform)
    {
        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
        Rigidbody.position = transform.position;
    }

    public void KnockBack(Transform instigator, float knockbackForce = 2000f)
    {
        Vector3 knockBackDirection = transform.position - instigator.position;
        Rigidbody.AddForce(knockBackDirection.normalized * knockbackForce);
    }

    public void Dash(Vector3 direction, float dashForce = 2000f)
    {
        if (direction == Vector3.zero)
            Rigidbody.AddForce(transform.forward.normalized * dashForce);
        else
            Rigidbody.AddForce(direction.normalized * dashForce);
    }

    public void ResetSpeed() => CurrentSpeedMultiplier = 1f;
    public void SetSpeedMultiplier(float multiplier)
    {
        CurrentSpeedMultiplier = multiplier;
    }


    public void SetMoveInput(Vector3 input)
    {
        MoveInput = input;
    }

    protected override void Update()
    {
        base.Update();
        
    }
}
