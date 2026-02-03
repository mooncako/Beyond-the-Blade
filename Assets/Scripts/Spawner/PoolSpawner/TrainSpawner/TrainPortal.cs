using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrainPortal : PoolSpawner
{
    [SerializeField, BoxGroup("References")] private LevelMechanicTrain _train;
    [SerializeField, BoxGroup("References")] private Transform _startPoint;
    [SerializeField, BoxGroup("References")] private Transform _endPoint;
    [SerializeField, BoxGroup("Settings")] private float _moveDuration = 2f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _stopPoint;

    private Tween _moveTween;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_train == null) _train = GetComponentInChildren<LevelMechanicTrain>();
        _stopPoint = new Vector3((_startPoint.position.x + _endPoint.position.x) / 2f, _train.transform.position.y, ( _startPoint.position.z + _endPoint.position.z) / 2f);
    }

    void Start()
    {
        _stopPoint = new Vector3((_startPoint.position.x + _endPoint.position.x) / 2f, _train.transform.position.y, ( _startPoint.position.z + _endPoint.position.z) / 2f);
    }

    void OnEnable()
    {
        if(_spawnTriggerType == EnemySpawnTriggerType.Collision)
            _spawnTrigger.TriggerEnter.AddListener(OnTrigger);
        
        _train.OnTrainArrived.AddListener(OnTrainGateOpen);
        _train.OnGateClosed.AddListener(OnGateClosed);
    }

    void OnDisable()
    {
        if(_spawnTriggerType == EnemySpawnTriggerType.Collision)
            _spawnTrigger.TriggerEnter.RemoveListener(OnTrigger);
        
        _train.OnTrainArrived.RemoveListener(OnTrainGateOpen);
        _train.OnGateClosed.RemoveListener(OnGateClosed);
        _moveTween.Stop();
    }

    public void OnTrigger(Collider other)
    {
        MoveTrainToMid();
    }

    private void MoveTrainToMid()
    {
        _train.EnableShadow();
        _moveTween.Stop();
        _moveTween = Tween.Position(_train.transform, _stopPoint, _moveDuration).OnComplete(() =>
        {
            _train.OpenGate();
        });
    }

    private void OnTrainGateOpen()
    {
        SpawnEnemies();
        Tween.Delay(.5f).OnComplete(() =>
        {
            _train.CloseGate();
        }
        );
    }

    private void OnGateClosed()
    {
        _moveTween.Stop();
        _moveTween = Tween.Position(_train.transform, _endPoint.position, _moveDuration).OnComplete(() =>
        {
            OnSpawnCompleted.Invoke(null);
            _train.DisableShadow();
        });
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_stopPoint, .5f);
        Gizmos.DrawLine(_stopPoint, new Vector3(_stopPoint.x, _stopPoint.y+1f, _stopPoint.z));
    }

    
}
