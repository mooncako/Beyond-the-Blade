using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField, HideInInspector] public float Range = 4f;
    [SerializeField, HideInInspector] public float AlertRange = 6f;
    [SerializeField, HideInInspector] public float FieldOfView = 140f;
    [SerializeField] private LayerMask _occlusionMask;      // blocks visibility, usually walls
    [SerializeField] private LayerMask _visibilityMask;     // visible objects

    // useful properties
    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;
    public Vector3 AimPosition => transform.position + new Vector3(0f, 1f, 0f);

    public void ApplyStats(Stats stats)
    {
        Range = stats.StatsData.Range;
        AlertRange = stats.StatsData.AlertRange;
        FieldOfView = stats.StatsData.FieldOfView;
    }

    public void SetupMasks(LayerMask visibilityMask, LayerMask occlusionMask)
    {
        _visibilityMask = visibilityMask;
        _occlusionMask = occlusionMask;
    }

    public bool TestVisibility(Vector3 point)
    {
        // distance
        float distance = Vector3.Distance(AimPosition, point);
        if (distance > Range) return false;

        // angle
        Vector3 dirToPoint = (point - AimPosition).normalized;
        float angleToPoint = Vector3.Angle(Forward, dirToPoint);
        float viewHalfAngle = FieldOfView * 0.5f;
        if (angleToPoint > viewHalfAngle) return false;

        // occlusion
        // LineCast checks collision between two points, we don't need direction or distance
        if (Physics.Linecast(AimPosition, point, _occlusionMask)) return false;

        // passed all tests, point is visible
        return true;
    }

    public bool TestTargetVisibility(int team)
    {
        return GetVisibleTargets(team).Count > 0;
    }

    public List<Targetable> GetVisibleTargets(int team)
    {
        List<Targetable> targets = new List<Targetable>();

        Collider[] hits = Physics.OverlapSphere(AimPosition, Range, _visibilityMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;                   // skip ourselves
            if (!hit.TryGetComponent(out Targetable target)) continue;    // skip objects without Targetable
            if (target.Team == team) continue;                            // skip allies
            if (!target.IsTargetable) continue;                           // skip not targetable
            if (!TestVisibility(target.transform.position)) continue;     // skip not visible

            // all tests passed, add target to list
            targets.Add(target);
        }

        return targets;
    }

    public List<Targetable> GetAlertTargets(int team)
    {
        List<Targetable> targets = new List<Targetable>();

        Collider[] hits = Physics.OverlapSphere(AimPosition, AlertRange, _visibilityMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;                   // skip ourselves
            if (!hit.TryGetComponent(out Targetable target)) continue;    // skip objects without Targetable
            if (target.Team == team) continue;                            // skip allies
            if (!target.IsTargetable) continue;                           // skip not targetable

            // all tests passed, add target to list
            targets.Add(target);
        }

        return targets;
    }

    public Targetable GetFirstVisibleTarget(int team)
    {
        List<Targetable> targets = GetVisibleTargets(team);
        if (targets.Count == 0) return null;
        return targets[0];

        // more sophisticated AI could use tunable heuristics to make more intelligent decisions:
        // ec: distance, path distance, direction, target aim direction, weaknesses, abilities, health, threat
    }

    // draws visible gizmos in scene/game view, but only when GameObject is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(AimPosition, Forward * AlertRange);
        float viewHalfAngle = FieldOfView * 0.5f;
        Quaternion forwardRotation = transform.rotation;
        Quaternion rotateLeft = Quaternion.Euler(0f, -viewHalfAngle, 0f) * forwardRotation;
        Quaternion rotateRight = Quaternion.Euler(0f, viewHalfAngle, 0f) * forwardRotation;
        Vector3 dirLeft = rotateLeft * Vector3.forward;
        Vector3 dirRight = rotateRight * Vector3.forward;
        Gizmos.DrawRay(AimPosition, dirLeft * AlertRange);
        Gizmos.DrawRay(AimPosition, dirRight * AlertRange);
    }
}
