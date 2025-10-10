using UnityEngine;

public class EnemySpawnPos : DrawPos
{
    [SerializeField] public float Radius = 1.5f;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
