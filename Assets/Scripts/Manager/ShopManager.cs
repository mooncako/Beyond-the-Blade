using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
public class ShopManager : MMSingleton<ShopManager>
{

    [Header("Shop Configruation")]
    [SerializeField] private ShopItemDatabaseSO _itemDatabase;
    [SerializeField] private int _itemPerShop = 5;
    [SerializeField] private int _baseItemCost = 10;

    [Header("Current Shop State")]
    [SerializeField, ReadOnly] private List<ShopItemSO> _currentItems;
    [SerializeField, ReadOnly] private bool _isShopOpen = false;
    [SerializeField, ReadOnly] private int _playerCoins;


    public List<ShopItemSO> CurrentItems => _currentItems;
    public bool IsShopOpen => _isShopOpen;
    public int PlayerCoins => _playerCoins;

    [FoldoutGroup("Events")] public UnityEvent<ShopItemSO> OnItemPurchased;
    [FoldoutGroup("Events")] public UnityEvent OnShopOpen;
    [FoldoutGroup("Events")] public UnityEvent OnShopClose;
    [FoldoutGroup("Events")] public UnityEvent<int> OnPlayerCoinsChanged;

    public void OpenShop()
    {
               
    }

    public void CloseShop()
    {
        
    }

    public bool TryPurchaseItem(ShopItemSO item)
    {
        return false;
    }

    public void GenerateShopItems()
    {

    }

    public void AddPlayerCoins(int amount)
    {

    }


}
