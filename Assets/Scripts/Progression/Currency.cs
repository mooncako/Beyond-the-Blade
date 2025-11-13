using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
public abstract class Currency : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected SphereCollider _collider;
    [SerializeField, BoxGroup("References")] protected VisualEffect _currencyVfx;
    [SerializeField, BoxGroup("References")] protected VisualEffect _onHitVfx;
    [SerializeField, BoxGroup("References")] protected VFXFinishedEventHandler _finishedHandler;

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
    }

    protected virtual void OnEnable()
    {
        if(_currencyVfx != null)
        {
            _currencyVfx.Play();
        }

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

}
