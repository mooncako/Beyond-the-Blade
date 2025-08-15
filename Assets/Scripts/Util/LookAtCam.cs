using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    [SerializeField] private bool _lockX = false;
    [SerializeField] private float _offset = 0f;

    private void Update()
    {
        if(!_lockX)
            transform.LookAt(Camera.main.transform);
        else
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.x = 0;
            if(direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, targetRotation.eulerAngles.y + _offset, targetRotation.eulerAngles.z);
            }
        }
    }
}
