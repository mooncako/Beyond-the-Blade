using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] protected Image _healthImage;
    //[BoxGroup("References"), SerializeField] protected Health _health;

    protected virtual void Start()
    {
       
    }
    protected virtual void OnDisable()
    {

    }
    public virtual void OnDamage(float healthPercentage)
    {
        _healthImage.fillAmount = healthPercentage;
    }
}
