using UnityEngine;

public class SpawnPos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 255, 127);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + .9f, transform.position.z), new Vector3(.5f, 1.8f, .5f));
    }
}
