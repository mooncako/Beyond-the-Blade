using System;
using Animancer;
using CharacterMovement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class CustomCharacterMovement : CharacterMovement3D
{
    [Header("References")] 
    [SerializeField] private CustomTimeScale _timeScale;
    
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
        
        if(_timeScale == null) _timeScale = GetComponent<CustomTimeScale>();

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

    protected override void FixedUpdate()
    {
        // check for the ground
        IsGrounded = CheckGrounded();

        // overrides current input with pathing direction if MoveTo has been called
        if (NavMeshAgent.hasPath && NavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            Vector3 nextPathPoint = NavMeshAgent.steeringTarget;
            Vector3 lastPathPoint = NavMeshAgent.path.corners[NavMeshAgent.path.corners.Length - 1];
            float lastPointDistance = Vector3.Distance(lastPathPoint, transform.position);
            bool pathEndReached = lastPointDistance < StoppingDistance;
            Vector3 pathDir = (nextPathPoint - transform.position).normalized;
            // override direction if avoidance is enabled
            if (EnableAvoidance)
            {
                float neighborDistance = NeighborDistance;
                if (NavMeshAgent.path.corners.Length > 2) neighborDistance = CornerNeighborDistance;
                pathDir = GetAvoidanceDirection(nextPathPoint, neighborDistance);

                if (IsClampedToNavMesh)
                {
                    Vector3 pathPoint = transform.position + pathDir * Speed * ClampLookAheadTime;
                    Vector3 clampedPathPoint = ClampToNavMesh(pathPoint, ClampSearchRadius);
                    pathDir = (clampedPathPoint - transform.position).normalized;
                }
            }

            SetMoveInput(pathDir);
            if (LookInMoveDirection) SetLookDirection(pathDir);

            bool destinationReached = Vector3.Distance(NavMeshAgent.destination, transform.position) < StoppingDistance;
            // stop off destination reached
            if (pathEndReached || (StoppingDistance > 0f && destinationReached))
            {
                SetLookPosition(NavMeshAgent.destination);
                Stop();
            }
        }

        // syncs navmeshagent position with character position
        NavMeshAgent.nextPosition = transform.position;
        NavMeshAgent.Warp(transform.position);

        // find flattened movement vector based on ground normal
        Vector3 input = MoveInput;
        Vector3 right = Vector3.Cross(transform.up, input);
        Vector3 forward = Vector3.Cross(right, GroundNormal);

        // move character along spline
        if (EnableSplineConstraint && SplineContainer != null)
        {
            // spline closest point and tangent
            Spline spline = SplineContainer.Spline;
            Vector3 splineRelativePosition = SplineContainer.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(spline, splineRelativePosition, out float3 nearest, out float t);
            Vector3 splineWorldPosition = SplineContainer.transform.TransformPoint(nearest);
            Vector3 splineTangent = SplineUtility.EvaluateTangent(spline, t);
            splineTangent.y = 0f;
            splineTangent.Normalize();

            // float direction to closest point
            Vector3 dirToSplineCenter = splineWorldPosition - transform.position;
            dirToSplineCenter.y = 0f;
            float splineFlatDistance = dirToSplineCenter.magnitude;
            dirToSplineCenter.Normalize();

            // force bringing character back to spline center
            float gravitationDot = Vector3.Dot(splineTangent, dirToSplineCenter);
            float gravitationCorrection = 1f - Math.Abs(gravitationDot);
            float sideInput = Vector3.Dot(MoveInput, splineTangent);
            Rigidbody.AddForce(gravitationCorrection * Mathf.Clamp01(splineFlatDistance) * SplineGravitation *
                               dirToSplineCenter);

            // correct movement direction along spline
            forward = MoveInput.magnitude * sideInput * splineTangent;
            SplineLookDirection = splineTangent * Mathf.Sign(sideInput);
        }

        // vary character speed when using avoidance
        float speed = Speed;
        if (EnableAvoidance)
        {
            float noise = Mathf.PerlinNoise(Time.time, _variationNoiseOffset) * 2f - 1f;
            speed = Speed * (1f + noise * SpeedVariation);
        }

        // calculates desirection movement velocity
        Vector3 targetVelocity = forward * (speed * MoveSpeedMultiplier * _timeScale.CurrentTimeScale);
        if (!CanMove) targetVelocity = Vector3.zero;
        // adds velocity of surface under character, if character is stationary
        targetVelocity += SurfaceVelocity * (1f - Mathf.Abs(MoveInput.magnitude));
        // calculates acceleration required to reach desired velocity and applies air control if not grounded
        Vector3 velocityDiff = targetVelocity - Velocity;
        velocityDiff.y = 0f;
        float control = IsGrounded ? 1f : AirControl;
        Vector3 acceleration = velocityDiff * (Acceleration * control);
        // zeros acceleration if airborne and not trying to move (allows for nice jumping arcs)
        if (!IsGrounded && !HasMoveInput) acceleration = Vector3.zero;
        // add gravity
        acceleration += GroundNormal * Gravity;

        Rigidbody.AddForce(acceleration * Rigidbody.mass);

        StepCheck();
    }

    protected override void Update()
    {
        // rotates character towards movement direction
        if (ControlRotation && (HasTurnInput || !OnlyTurnWithInput) && (IsGrounded || AirTurning))
        {
            Quaternion rotation = Rigidbody.rotation;
            if (!Fix3DSpriteRotation)
            {
                if (EnableSplineConstraint && HasMoveInput) LookDirection = SplineLookDirection;
                Quaternion targetRotation = Quaternion.LookRotation(LookDirection);
                rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    TurnSpeed * TurnSpeedMultiplier * Time.deltaTime * _timeScale.CurrentTimeScale);
            } // rotate sprite character properly
            else if (Fix3DSpriteRotation && Mathf.Abs(MoveInput.x) > 0.2f)
            {
                float spriteAngle = LookDirection.x > 0 ? 0f : 180f;
                rotation = Quaternion.Euler(0f, spriteAngle, 0f);
            }

            Rigidbody.MoveRotation(rotation);
            transform.rotation = rotation;
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
}
