using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;
public class ShopUI : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private Canvas _shopItemUIGroupCanvas;
    [SerializeField] private Canvas _backgroundCanvas;
    [SerializeField] private GameObject _shopItemUIPrefab;
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    [SerializeField] private Button _cloaseButton;
    [Header("Debug")]
    public bool ToggleDebug = false;
    

    private List<ShopItemUI> _currentItemUIs = new List<ShopItemUI>();



    private void OnEnable()
    {
      
        ShopManager.Instance.OnShopOpened.AddListener(ShowShop);
        ShopManager.Instance.OnShopClosed.AddListener(HideShop);
        ShopManager.Instance.OnPlayerCoinsChanged.AddListener(UpdateCoinsDisplay);
        ShopManager.Instance.OnItemPurchased.AddListener(OnItemPurchased);
            
        
        
        // Set up button listener
        if (_cloaseButton != null)
        {
            _cloaseButton.onClick.AddListener(HideShop);
        }
    }

    private void OnDisable()
    {
     
        ShopManager.Instance.OnShopOpened.RemoveListener(ShowShop);
        ShopManager.Instance.OnShopClosed.RemoveListener(HideShop);
        ShopManager.Instance.OnPlayerCoinsChanged.RemoveListener(UpdateCoinsDisplay);
        ShopManager.Instance.OnItemPurchased.RemoveListener(OnItemPurchased);
        if (_cloaseButton != null)
        {
            _cloaseButton.onClick.RemoveAllListeners();
        }
    }
    private void Start()
    {
        // Initially hide the shop UI
        HideShop();
    }

    [Button("ShowShop")]
    private void ShowShop()
    {
        _shopItemUIGroupCanvas.gameObject.SetActive(true);
        _backgroundCanvas.gameObject.SetActive(true);
        
        // Only create UI elements if we're in play mode and ShopManager exists
        if (Application.isPlaying && ShopManager.Instance != null)
        {
            CreateItemUIs();
            UpdateCoinsDisplay(ShopManager.Instance.PlayerCoins);
        }
    }

    [Button("HideShop")]
    private void HideShop()
    {
        _shopItemUIGroupCanvas.gameObject.SetActive(false);
        _backgroundCanvas.gameObject.SetActive(false);

    }
    
    private void UpdateCoinsDisplay(int coins)
    {
        _totalCoinsText.text = $"Coins: {coins}";
    }

    private void OnItemPurchased(ShopItemSO item)
    {
        CreateItemUIs();
    }

    private void CreateItemUIs()
    {
        // Safety check - don't run in editor or if ShopManager doesn't exist
        if (!Application.isPlaying || ShopManager.Instance == null)
            return;

        // Clear existing items
        foreach(var itemUI in _currentItemUIs)
        {
            if(itemUI != null)
                Destroy(itemUI.gameObject);
        }
        _currentItemUIs.Clear();

        // Create new item UIs
        foreach(var item in ShopManager.Instance.CurrentItems)
        {
            // Parent is already set in Instantiate, no need to call SetParent again
            var itemUIObject = Instantiate(_shopItemUIPrefab, _shopItemUIGroupCanvas.transform);
            var itemUI = itemUIObject.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item);
                _currentItemUIs.Add(itemUI);
            }
            else
            {
                Debug.LogError("ShopItemUI component not found on prefab!");
                Destroy(itemUIObject);
            }
        }
    }


}
