using System.Runtime.CompilerServices;
using UnityEngine;


[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("Item Info")]
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public int Cost;

    [Header("Setting")]
    public bool CanPurchaseMultiple= false;
    public int MaxPurchaseCount = 1;

    public void ApplyEffect()
    {
        Debug.Log($"Applying effect of {ItemName}");
    }
}
