using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private PlayerController _playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnValidate()
    {
        if (_playerController == null) _playerController = GetComponentInParent<PlayerController>();
    }
    void Start()
    {
    }

    void OnEnable()
    {
        if (_playerController != null)
            _playerController.OnAbilityStartCooldown.AddListener(OnAbilityCooldownStarted);
    }
    void OnDisable()
    {
        if (_playerController != null)
            _playerController.OnAbilityStartCooldown.RemoveListener(OnAbilityCooldownStarted);
    }
    
    private void OnAbilityCooldownStarted()
    {
        StartCoroutine(StartIconCooldownEffect());
    }
    
    private IEnumerator StartIconCooldownEffect()
    {
        if (_playerController.CurrentAbility == null) 
        {
            yield break;
        }
        
        float cooldownTime = _playerController.CurrentAbility.Cooldown;
        float elapsedTime = 0f;
        
        icon.fillAmount = 0f;
        
        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            icon.fillAmount = elapsedTime / cooldownTime;
            yield return null;
        }
        
        icon.fillAmount = 1f;
    }
}
