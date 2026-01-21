using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrainPortal : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private LevelMechanicTrain _train;
    [SerializeField, BoxGroup("References")] private EnemyPool _enemyPool;
    [SerializeField, BoxGroup("References")] private Transform _startPoint;
    [SerializeField, BoxGroup("References")] private Transform _endPoint;
    [SerializeField, BoxGroup("References")] private CollisionTrigger _portalTrigger;
    [SerializeField, BoxGroup("Settings")] private float _moveDuration = 2f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _stopPoint;

    private Tween _moveTween;

    void OnValidate()
    {
        if (_portalTrigger == null) _portalTrigger = GetComponentInChildren<CollisionTrigger>();
        if (_train == null) _train = GetComponentInChildren<LevelMechanicTrain>();
        if (_enemyPool == null) _enemyPool = GetComponentInChildren<EnemyPool>();
        _stopPoint = new Vector3((_startPoint.position.x + _endPoint.position.x) / 2f, _train.transform.position.y, ( _startPoint.position.z + _endPoint.position.z) / 2f);
    }

    void Start()
    {
        _stopPoint = new Vector3((_startPoint.position.x + _endPoint.position.x) / 2f, _train.transform.position.y, ( _startPoint.position.z + _endPoint.position.z) / 2f);
    }

    void OnEnable()
    {
        _portalTrigger.TriggerEnter.AddListener(OnTrigger);
        _train.OnTrainArrived.AddListener(OnTrainGateOpen);
    }

    void OnDisable()
    {
        _portalTrigger.TriggerEnter.RemoveListener(OnTrigger);
        _train.OnTrainArrived.RemoveListener(OnTrainGateOpen);
        _moveTween.Stop();
    }

    private void OnTrigger(Collider other)
    {
        MoveTrainToMid();
    }

    private void MoveTrainToMid()
    {
        _moveTween.Stop();
        _moveTween = Tween.Position(_train.transform, _stopPoint, _moveDuration).OnComplete(() =>
        {
            _train.OpenGate();
        });
    }

    private void OnTrainGateOpen()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_stopPoint, .5f);
        Gizmos.DrawLine(_stopPoint, new Vector3(_stopPoint.x, _stopPoint.y+1f, _stopPoint.z));
    }
}
