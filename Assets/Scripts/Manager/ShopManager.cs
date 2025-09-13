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
    [FoldoutGroup("Events")] public UnityEvent OnShopOpened;
    [FoldoutGroup("Events")] public UnityEvent OnShopClosed;
    [FoldoutGroup("Events")] public UnityEvent<int> OnPlayerCoinsChanged;



    protected override void Awake()
    {
        base.Awake();
        
        // Only initialize if this is the main instance
        if (this == Instance)
        {
            GenerateShopItems();
            DontDestroyOnLoad(this);
            _playerCoins = 100;
        }
    }
    public void OpenShop()
    {
        if(_isShopOpen)
            return;
        _isShopOpen = true;
        OnShopOpened?.Invoke();

    }

    public void CloseShop()
    {
        if(!_isShopOpen)
            return;
        _isShopOpen = false;
        OnShopClosed?.Invoke();

    }

    public bool TryPurchaseItem(ShopItemSO item)
    {
        if(item == null)
            return false;
        if(_playerCoins < item.Cost)
            return false;
        _playerCoins -= item.Cost;
        item.ApplyEffect();
        OnPlayerCoinsChanged?.Invoke(_playerCoins);
        OnItemPurchased?.Invoke(item);
        return true;
    }

    public void GenerateShopItems()
    {
        _currentItems = new List<ShopItemSO>();
        
        // Safety check
        if (_itemDatabase == null || _itemDatabase.ShopItems == null)
        {
            Debug.LogWarning("ShopManager: ItemDatabase or ShopItems is null!");
            return;
        }
        
        for(int i = 0; i < _itemPerShop && i < _itemDatabase.ShopItems.Count; i++)
        {
            if (_itemDatabase.ShopItems[i] != null)
                _currentItems.Add(_itemDatabase.ShopItems[i]);
        }
    }

    public void AddPlayerCoins(int amount)
    {

    }


}
