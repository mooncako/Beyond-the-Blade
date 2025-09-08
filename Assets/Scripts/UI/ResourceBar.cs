using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using PrimeTween;
public class ResourceBar : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] protected Image _resourceImage;
    private Tween _fillTween;

    protected virtual void Start()
    {
       
    }
    protected virtual void OnDisable()
    {
        _fillTween.Stop();
    }
    
    public virtual void UpdateFillAmount(float fillAmount)
    {
        _fillTween.Stop();
        float currentAmount = _resourceImage.fillAmount;
        _fillTween = Tween.Custom(currentAmount, fillAmount, duration: .5f, onValueChange: fill => _resourceImage.fillAmount = fill);
    }
}
