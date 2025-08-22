using UnityEngine;

public class ExitPos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 102, 0);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + .9f, transform.position.z), new Vector3(.5f, 1.8f, .5f));
    }
}
