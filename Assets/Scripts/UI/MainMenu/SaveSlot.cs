using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _saveName;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public bool IsSaved;

    [HideInInspector] public UnityEvent<SaveSlot> OnSaveSlotSelected;

    void OnValidate()
    {
        if (_saveName == null) _saveName = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSaveSlotSelected.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void ToggleSave(bool toggle, string name)
    {
        IsSaved = toggle;
        _saveName.text = name;
    }

    public string GetSaveName()
    {
        return _saveName.text;
    }
}
