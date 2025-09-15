using Animancer;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilitySwapUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _abilityName;
    [HideInInspector] public UnityEvent<int> OnClick;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _index;

    void OnValidate()
    {
        if (_abilityName == null) _abilityName = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void AssignData(Skill skill, int index)
    {
        _abilityName.text = skill.Name;
        _index = index;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        
    }

    public void OnPointerExit(PointerEventData e)
    {
        
    }

    public void OnPointerClick(PointerEventData e)
    {
        OnClick.Invoke(_index);
    }
}
