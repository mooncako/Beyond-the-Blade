using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class ResourceBar : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] protected Image _resourceImage;
    //[BoxGroup("References"), SerializeField] protected Health _health;

    protected virtual void Start()
    {
       
    }
    protected virtual void OnDisable()
    {

    }
    public virtual void UpdateFillAmount(float fillAmount)
    {
        _resourceImage.fillAmount = fillAmount;
    }
}
