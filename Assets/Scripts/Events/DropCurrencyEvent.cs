using MoreMountains.Tools;
using UnityEngine;

public struct DropCurrencyEvent
{
    public CurrencyType CurrencyType;
    public int Amount;
    public Vector3 Pos;

    public DropCurrencyEvent(CurrencyType currencyType, int amount, Vector3 pos)
    {
        CurrencyType = currencyType;
        Amount = amount;
        Pos = pos;
    }

    public static DropCurrencyEvent e;
    public static void Trigger(CurrencyType currencyType, int amount, Vector3 pos)
    {
        e.CurrencyType = currencyType;
        e.Amount = amount;
        e.Pos = pos;
        MMEventManager.TriggerEvent(e);
    }
}
