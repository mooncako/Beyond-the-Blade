
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _name;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _damage;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _desc;
    [SerializeField, BoxGroup("References")] private Image[] _rarity;
    [SerializeField, BoxGroup("References")] private Image _skillIcon;
    [SerializeField, BoxGroup("Settings")] private int _index;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public string _skillId;

    void OnValidate()
    {
        if (_name == null) _name = GetComponentsInChildren<TextMeshProUGUI>()[0];
        if (_damage == null) _damage = GetComponentsInChildren<TextMeshProUGUI>()[1];
        if (_desc == null) _desc = GetComponentsInChildren<TextMeshProUGUI>()[2];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AddNewAbilityEvent.Trigger(_skillId, _index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void AssignData(Skill skill, string SkillId)
    {
        _name.text = skill.Name;
        _desc.text = skill.Description;
        _damage.text = $"{skill.Damage}";
        _skillIcon.sprite = skill.Icon;
        for(int i = 0; i < _rarity.Length; i++)
        {
            _rarity[i].color = RarityUtil.GetRarityColor(skill.Rarity);
        }
        
        _skillId = SkillId;
    }
}
