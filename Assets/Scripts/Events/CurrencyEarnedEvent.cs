using MoreMountains.Tools;
using UnityEngine;

public struct CurrencyEarnedEvent
{
    public int Amount;
    public CurrencyType CurrencyType;

    public CurrencyEarnedEvent(int amount, CurrencyType currencyType)
    {
        Amount = amount;
        CurrencyType = currencyType;
    }

    public static CurrencyEarnedEvent e;
    public static void Trigger(int amount, CurrencyType currencyType)
    {
        e.Amount = amount;
        e.CurrencyType = currencyType;
        MMEventManager.TriggerEvent(e);
    }
}
