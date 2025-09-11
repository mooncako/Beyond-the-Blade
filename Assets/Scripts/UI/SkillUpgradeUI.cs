using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SkillUpgradeUI : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _attackOneText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _attackTwoText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _attackThreeText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _abilityText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _executionText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _parryText;
    [SerializeField, BoxGroup("References")] private TextMeshProUGUI _dashText;
}
