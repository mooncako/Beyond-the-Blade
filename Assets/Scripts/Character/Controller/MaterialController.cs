using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SkinnedMeshRenderer[] _skinnedMeshes;
    [SerializeField, BoxGroup("References")] private Health _health;

    private Tween _delayTween;

    void OnValidate()
    {
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (_health == null) _health = GetComponentInParent<Health>();
    }

    void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamage.AddListener(OnDamage);
        }
    }

    void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamage.RemoveListener(OnDamage);
        }
        _delayTween.Stop();
    }

    private void OnDamage(DamageInfo info)
    {
        _delayTween.Stop();
        Debug.Log(1);
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_OnDamage", 1);
            }
        }

        _delayTween = Tween.Delay(.15f).OnComplete(Recover);
    }

    private void Recover()
    {
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_OnDamage", 0);
            }
        }
    }

}
