using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class CharacterProjectile : BaseProjectile
{
    [SerializeField, BoxGroup("Visual Effects")] private VisualEffect _projectileVFX;
    [SerializeField, BoxGroup("Visual Effects")] private VisualEffect _onDestroyVFX;

    protected override void HandleTargetHit(GameObject target)
    {
        Health targetHealth = target.GetComponent<Health>();
        DamageInfo damageInfo = new DamageInfo(
            _data.damage,
            target,
            targetHealth,
            Owner,
            _data.damageType
        );

        if (targetHealth != null)
        {
            targetHealth.Damage(damageInfo);
        }

        // Trigger hit event
        OnTargetHit?.Invoke(target);

        // Increment hit count
        _hitCount++;

        // Check if projectile should be destroyed after hit
        if (!_data.isPiercing || (_data.maxHits > 0 && _hitCount >= _data.maxHits))
        {
            DestroyProjectile();
        }
    }

    protected override void DestroyProjectile()
    {
        _collider.enabled = false;

        if (_projectileVFX != null)
        {
            _projectileVFX.Stop();
        }

        if (_onDestroyVFX != null)
        {
            _onDestroyVFX.Play();
            _onDestroyVFX.GetComponent<VFXFinishedEventHandler>().OnVfxFinished.AddListener(Deactivate);
        }
        else
        {
            Deactivate();
        }


    }

    [Button]
    private void Test()
    {
        Activate();
    }
}
