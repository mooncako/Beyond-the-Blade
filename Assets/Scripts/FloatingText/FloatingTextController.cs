using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class FloatingTextController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MMF_Player _mmfPlayer;
    [SerializeField] private Controller _controller;
    
    [Header("Settings")]
    [SerializeField] private bool _showDamageNumbers = true;
    [SerializeField] private bool _showHealingNumbers = true;
    [SerializeField] private Vector3 _textOffset = Vector3.up * 2f;
    
    [Header("Text Formatting")]
    [SerializeField] private string _damagePrefix = "";
    [SerializeField] private string _damageSuffix = "";
    [SerializeField] private string _healingPrefix = "+";
    [SerializeField] private string _healingSuffix = "";
    [SerializeField] private bool _roundToInt = true;
    
    [Header("Events")]
    public UnityEvent<string> OnFloatingTextPlay;
    public UnityEvent<DamageInfo> OnDamageFloatingText;
    public UnityEvent<float> OnHealingFloatingText;
    
   
    private void OnValidate()
    {
        if (_mmfPlayer == null)
        {
            _mmfPlayer = GetComponentInChildren<MMF_Player>();
        }
        if( _controller == null)
        {
            _controller = GetComponentInParent<Controller>();
        }
    }
    void OnEnable()
    {
        OnFloatingTextPlay.AddListener(PlayFloatingText);
        OnDamageFloatingText.AddListener(PlayDamageFloatingText);
        OnHealingFloatingText.AddListener(PlayHealingFloatingText);
        if(_controller != null && _controller.Health != null)
        {
            _controller.Health.OnDamage.AddListener(PlayDamageFloatingText);
            _controller.Health.OnHealthRecovery.AddListener(PlayHealingFloatingText);
        }
    }
    
    void OnDisable()
    {
        OnFloatingTextPlay.RemoveListener(PlayFloatingText);
        OnDamageFloatingText.RemoveListener(PlayDamageFloatingText);
        OnHealingFloatingText.RemoveListener(PlayHealingFloatingText);
        if(_controller != null && _controller.Health != null)
        {
            _controller.Health.OnDamage.RemoveListener(PlayDamageFloatingText);
            _controller.Health.OnHealthRecovery.RemoveListener(PlayHealingFloatingText);

        }
    }

    /// <summary>
    /// Play floating text with custom string
    /// </summary>
    public void PlayFloatingText(string text)
    {
        PlayFloatingTextAtPosition(text, this.transform.position);
    }
    
    /// <summary>
    /// Play floating text for damage taken
    /// </summary>
    public void PlayDamageFloatingText(DamageInfo damageInfo)
    {
        if (!_showDamageNumbers || damageInfo == null) return;
        
        // Format damage text
        float damage = damageInfo.Amount;
        string damageText = FormatDamageText(damage);
        
        // Play floating text at this manager's position
        PlayFloatingTextAtPosition(damageText, transform.position + _textOffset);
    }
    
    /// <summary>
    /// Play floating text for healing
    /// </summary>
    public void PlayHealingFloatingText(float healAmount)
    {
        if (!_showHealingNumbers) return;
        
        string healText = FormatHealingText(healAmount);
        Vector3 textPosition = transform.position + _textOffset;
        
        PlayFloatingTextAtPosition(healText, textPosition);
    }
    
    /// <summary>
    /// Play floating text at specific position
    /// </summary>
    public void PlayFloatingTextAtPosition(string text, Vector3 position)
    {
        if (_mmfPlayer == null) return;
        
        MMF_FloatingText floatingText = _mmfPlayer.GetFeedbackOfType<MMF_FloatingText>();
        if (floatingText != null)
        {
            floatingText.Value = text;
            _mmfPlayer.PlayFeedbacks(position);
        }
    }
    
    /// <summary>
    /// Format damage amount as text
    /// </summary>
    private string FormatDamageText(float damage)
    {
        string damageValue = _roundToInt ? Mathf.RoundToInt(damage).ToString() : damage.ToString("F1");
        return $"{_damagePrefix}{damageValue}{_damageSuffix}";
    }
    
    /// <summary>
    /// Format healing amount as text
    /// </summary>
    private string FormatHealingText(float healing)
    {
        string healValue = _roundToInt ? Mathf.RoundToInt(healing).ToString() : healing.ToString("F1");
        return $"{_healingPrefix}{healValue}{_healingSuffix}";
    }
    
    
    [Button("Test Damage Text"), BoxGroup("Debug")]
    public void TestDamageText()
    {
        DamageInfo testDamage = new DamageInfo(25f, gameObject, null, gameObject, DamageType.Regular);
        PlayDamageFloatingText(testDamage);
    }
    
    [Button("Test Healing Text"), BoxGroup("Debug")]
    public void TestHealingText()
    {
        PlayHealingFloatingText(15f);
    }
}
