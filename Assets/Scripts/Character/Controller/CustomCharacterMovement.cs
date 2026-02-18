using Animancer;
using CharacterMovement;
using UnityEngine;
using UnityEngine.AI;

public class CustomCharacterMovement : CharacterMovement3D
{
    [Header("Character Setup")]
    [SerializeField] private bool _isHumanoid = true;

    protected override void OnValidate()
    {
        if (Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        if (NavMeshAgent == null)
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (CapsuleCollider == null)
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        Rigidbody.freezeRotation = true;
        Rigidbody.useGravity = false;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        NavMeshAgent = GetComponent<NavMeshAgent>();
        if(_isHumanoid)
        {
            NavMeshAgent.height = base.Height;
            NavMeshAgent.radius = base.Radius;
            CapsuleCollider.height = base.Height;
            CapsuleCollider.center = new Vector3(0f, base.Height * 0.5f, 0f);
            CapsuleCollider.radius = base.Radius;
        }
        
    }


    public void Teleport(Vector3 position)
    {
        transform.position = position;
        Rigidbody.position = position;
        NavMeshAgent.Warp(position);
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

    public void ResetSpeed() => MoveSpeedMultiplier = 1f;
    public void SetSpeedMultiplier(float multiplier)
    {
        MoveSpeedMultiplier = multiplier;
    }


    public override void SetMoveInput(Vector3 input)
    {
        MoveInput = input;
    }

    public bool IsAgentMoving()
    {
        return NavMeshAgent.hasPath || NavMeshAgent.velocity.magnitude > 0;
    }

    protected override void Update()
    {
        base.Update();
        
    }
}
