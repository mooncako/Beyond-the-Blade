using FMODUnity;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(DontDestroy))]
public abstract class Currency : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] protected SphereCollider _collider;
    [SerializeField, BoxGroup("References")] protected VisualEffect _currencyVfx;
    [SerializeField, BoxGroup("References")] protected VisualEffect _onHitVfx;
    [SerializeField, BoxGroup("References")] protected VFXFinishedEventHandler _finishedHandler;
    [SerializeField, BoxGroup("Settings")] protected LayerMask _playerMask;

    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _initialized = false;

    [HideInInspector] public UnityEvent OnCurrencyHit;

    public int Amount = 3;

    protected Tween _delayTween;

    protected virtual void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }

        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    protected virtual void OnEnable()
    {

        if(_finishedHandler != null)
        {
            _finishedHandler.OnVfxFinished.AddListener(OnHit);
        }
    }

    protected virtual void OnDisable()
    {
        if(_finishedHandler != null)
        {
            _finishedHandler.OnVfxFinished.RemoveListener(OnHit);
        }

        _delayTween.Stop();
    }

    protected virtual void OnHit()
    {
        
    }

    public virtual void OnPoolGet()
    {
        if(_currencyVfx != null)
        {
            _currencyVfx.Play();
        }
        _initialized = true;
    }

    public virtual void OnPoolReturn()
    {
        _collider.enabled = true;
    }
}
