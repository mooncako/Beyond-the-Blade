using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AnimationBasedEnemySpawner : EnemySpawner
{ 
    [SerializeField, BoxGroup("References")] private AnimancerComponent _animancer;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleDeathAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _spawnAnimation;
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    


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
        _collider.enabled = false;
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
        _spawnedEntity.transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        _collider.enabled = true;
    }

    protected override void SpawnEntity()
    {
        base.SpawnEntity();
    }

    public override void OnPoolGet()
    {
        base.OnPoolGet();
    }

    public override void OnPoolReturn()
    {
        base.OnPoolReturn();
        _animancer.gameObject.SetActive(true);
        _collider.enabled = false;
        _spawnedEntity.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
