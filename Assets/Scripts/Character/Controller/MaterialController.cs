using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SkinnedMeshRenderer[] _skinnedMeshes;
    [SerializeField, BoxGroup("References")] private Health _health;

    private Tween _delayTween;
    private Tween _iframeTween;

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
            _health.OnIframe.AddListener(OnIframe);
        }
    }

    void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamage.RemoveListener(OnDamage);
            _health.OnIframe.RemoveListener(OnIframe);
        }
        _delayTween.Stop();
        _iframeTween.Stop();
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_IsIframe", 0);
            }
        }
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_OnDamage", 0);
            }
        }
    }

    private void OnIframe(float duration)
    {
        _iframeTween.Stop();
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_IsIframe", 1);
            }
        }
        _delayTween = Tween.Delay(duration).OnComplete(() =>
        {
            foreach (var renderer in _skinnedMeshes)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetInt("_IsIframe", 0);
                }
            }
        });
    }

    private void OnDamage(DamageInfo info)
    {
        _delayTween.Stop();
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
