using UnityEngine;

public class BezierLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform startPoint;
    public Transform endPoint;



    [SerializeField] private float _height = 2.5f;
    void Start()
    {

    }

    void Update()
    {
        lineRenderer.positionCount = 50;
        for (int i = 0; i < 50; i++)
        {
            if(endPoint != null)
            {
                Vector3 point = BezierUtil.CalculateBezierPoint(startPoint.position, new Vector3(endPoint.position.x, endPoint.position.y + 2f
            , endPoint.position.z), i / 50.0f, _height);
                lineRenderer.SetPosition(i, point);
            }
            
        }
    }

}
