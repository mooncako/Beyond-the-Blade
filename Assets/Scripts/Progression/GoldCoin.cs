using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class GoldCoin : Currency
{

    void Update()
    {
        if(_initialized)
        {
            transform.position += (new Vector3(PlayerBroadcast.Instance.Players[0].transform.position.x, .9f, PlayerBroadcast.Instance.Players[0].transform.position.z) - transform.position).normalized * 10 * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(_currencyVfx != null)
        {
            _currencyVfx.gameObject.SetActive(false);
        }

        if(_onHitVfx != null)
        {
            _onHitVfx.Play();
        }
        _collider.enabled = false;
        _initialized = false;
    }


    protected override void OnHit()
    {
        base.OnHit();
        CurrencyEarnedEvent.Trigger(Amount, CurrencyType.Gold);
        _delayTween = Tween.Delay(1).OnComplete(() => 
        {
            OnCurrencyHit.Invoke();
            _currencyVfx.gameObject.SetActive(true);
        });
    }

    public override void OnPoolGet()
    {
        base.OnPoolGet();
    }

    public override void OnPoolReturn()
    {
        base.OnPoolReturn();
        
    }

    [Button]
    private void Test()
    {
        if(_currencyVfx != null)
        {
            _currencyVfx.Play();
        }
        _initialized = true;
    }
}
