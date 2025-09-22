using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _saveName;

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
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void AssignData(string name)
    {
        _saveName.text = name;
    }
}
