using UnityEngine;

public class DrawPos : MonoBehaviour
{
    [SerializeField] protected Color _color;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + .9f, transform.position.z), new Vector3(.5f, 1.8f, .5f));
    }

}
