using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopItemUI : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    private ShopItemSO _item;

    public void Initialize(ShopItemSO item)
    {
        _item = item;
        _iconImage.sprite = item.Icon;
        _nameText.text = item.ItemName;
        _descriptionText.text = item.Description;
        _costText.text = item.Cost.ToString();

        _purchaseButton.onClick.RemoveAllListeners();
        _purchaseButton.onClick.AddListener(TryPurchase);
    }


    public void TryPurchase()
    {
        if(ShopManager.Instance.TryPurchaseItem(_item))
        {
            Debug.Log($"Purchased {_item.ItemName}");
           _canvasGroup.alpha = 0.5f;
              _purchaseButton.interactable = false;
            _costText.text = "SOLD";
        }
    }


}
