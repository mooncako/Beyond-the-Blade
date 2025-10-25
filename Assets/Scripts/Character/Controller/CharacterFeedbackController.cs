using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterFeedbackController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Health _health;

    [HideInInspector, SerializeField] private LightShake _lightShake = new LightShake();

    void OnValidate()
    {
        if (_health == null) _health = GetComponentInParent<Health>();
    }

    void OnEnable()
    {
        _health.OnDamage.AddListener(PlayDamageFeedback);
        _health.OnDeath.AddListener(PlayDeathFeedback);
    }

    void OnDisable()
    {
        _health.OnDamage.RemoveListener(PlayDamageFeedback);
        _health.OnDeath.RemoveListener(PlayDeathFeedback);
    }

    private void PlayDamageFeedback(DamageInfo info)
    {
        CameraShakeEvent.Trigger(_lightShake);
    }
    
    private void PlayDeathFeedback(DamageInfo info)
    {
        LoadSceneEvent.Trigger("TestHub");
    }
}
