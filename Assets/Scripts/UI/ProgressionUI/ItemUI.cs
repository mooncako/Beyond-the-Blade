using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private Image _icon;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _title;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _desc;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private ShopItemSO _item;

    void OnValidate()
    {
        if (_title == null) _title = GetComponentsInChildren<TextMeshProUGUI>()[0];
        if (_desc == null) _desc = GetComponentsInChildren<TextMeshProUGUI>()[1];
    }

    public void AssignData(ShopItemSO item)
    {
        _item = item;
        _icon.sprite = item.Icon;
        _title.text = item.ItemName;
        _desc.text = item.Description;
    }

    public void OnPointerClick(PointerEventData e)
    {
        _item.ApplyEffect();
        NewItemSelectionEvent.Trigger(EventStateType.OnEventEnd);
        ProgressionCanvasCloseEvent.Trigger();
    }

    public void OnPointerExit(PointerEventData e)
    {
        
    }

    public void OnPointerEnter(PointerEventData e)
    {
        
    }
}
