using Animancer;
using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AnimationBasedEnemySpawner : EnemySpawner,
    MMEventListener<ManualSequentialEnemySpawnerTriggerEvent>
{ 
    [SerializeField, BoxGroup("References")] private AnimancerComponent _animancer;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleDeathAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _spawnAnimation;
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField, BoxGroup("References")] private LayerMask _playerMask;
    


    protected override void OnValidate()
    {
        base.OnValidate();
        if(_collider == null) 
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }
        if(_animancer == null) _animancer = GetComponentInChildren<AnimancerComponent>();
        _spawnAnimation.NormalizedStartTime = 0;
        _idleAnimation.NormalizedStartTime = 0;
        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    void Awake()
    {
        
    }

    void OnEnable()
    {
        _animancer.Play(_idleDeathAnimation);
    }

    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if(SpawnTriggerType == EnemySpawnTriggerType.Collision && (_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            _collider.enabled = false;
            TriggerAnimationSpawn();
        }
    }

    private void TriggerAnimationSpawn()
    {
        _animancer.Play(_spawnAnimation).Events(this).Add(1, () =>
        {
            _animancer.Play(_idleAnimation);
            Tween.Delay(.5f).OnComplete(() =>
            {
                SpawnEntity();
                _animancer.gameObject.SetActive(false);
            });
        });
    }

    public override void StartSpawn()
    {
        base.StartSpawn();
        float rotation = Random.Range(-180f, 180f);
        _spawnedEntity.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        _animancer.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        if(SpawnTriggerType == EnemySpawnTriggerType.Collision)
            _collider.enabled = true;
    }

    protected override void SpawnEntity()
    {
        base.SpawnEntity();
    }

    public override void OnPoolGet()
    {
        base.OnPoolGet();

        this.MMEventStartListening<ManualSequentialEnemySpawnerTriggerEvent>();
    }

    public override void OnPoolReturn()
    {
        base.OnPoolReturn();
        _animancer.gameObject.SetActive(true);
        _collider.enabled = false;
        _spawnedEntity.transform.localRotation = Quaternion.Euler(0, 0, 0);
        _animancer.transform.localRotation = Quaternion.Euler(0, 0, 0);

        this.MMEventStopListening<ManualSequentialEnemySpawnerTriggerEvent>();
    }

    public void OnMMEvent(ManualSequentialEnemySpawnerTriggerEvent e)
    {
        if(e.EncounterID == EncounterID && SpawnTriggerType == EnemySpawnTriggerType.Manual)
        {
            TriggerAnimationSpawn();
        }
    }
}
