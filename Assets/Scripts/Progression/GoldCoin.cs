using UnityEngine;

public class GoldCoin : Currency
{
    public int Amount = 3;

    void OnTriggerEnter(Collider other)
    {
        CurrencyEarnedEvent.Trigger(Amount, CurrencyType.Gold);
    }
}
