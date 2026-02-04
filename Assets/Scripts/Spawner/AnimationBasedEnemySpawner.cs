using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimationBasedEnemySpawner : EnemySpawner
{ 
    [SerializeField, BoxGroup("References")] private AnimancerComponent _animancer;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleDeathAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _idleAnimation;
    [SerializeField, BoxGroup("References")] private ClipTransition _spawnAnimation;
    


    protected override void OnValidate()
    {
        base.OnValidate();
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

    public override void StartSpawn()
    {
        base.StartSpawn();
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
    }
}
