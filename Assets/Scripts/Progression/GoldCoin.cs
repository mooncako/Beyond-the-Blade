using PrimeTween;
using UnityEngine;

public class GoldCoin : Currency
{

    void OnTriggerEnter(Collider other)
    {
        if(_currencyVfx != null)
        {
            _currencyVfx.Stop();
        }

        if(_onHitVfx != null)
        {
            _onHitVfx.Play();
        }
    }


    protected override void OnHit()
    {
        base.OnHit();
        CurrencyEarnedEvent.Trigger(Amount, CurrencyType.Gold);
        _delayTween = Tween.Delay(1).OnComplete(() => OnCurrencyHit.Invoke());
    }
}
